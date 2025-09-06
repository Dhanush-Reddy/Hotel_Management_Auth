import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { InvoicesService, InvoiceListItem, InvoiceStatus } from '../../services/invoices.service';
import { BookingsService } from '../../../bookings/services/bookings.service';
import { RoomsService } from '../../../rooms/services/rooms.service';
import { GuestsService } from '../../../guests/services/guests.service';

@Component({
  selector: 'app-invoices-list',
  templateUrl: './invoices-list.component.html',
  styleUrls: ['./invoices-list.component.scss']
})
export class InvoicesListComponent implements OnInit {
  data = new MatTableDataSource<InvoiceListItem>([]);
  cols = ['id','bookingId','guestName','roomNumber','totalAmount','status','issuedAt','actions'];
  q = ''; status: ''|InvoiceStatus = ''; loading = false;
  roomNo = '';

  private roomCache = new Map<number, string>();
  private guestCache = new Map<number, string>();

  constructor(
    private api: InvoicesService,
    private router: Router,
    private bookingsApi: BookingsService,
    private roomsApi: RoomsService,
    private guestsApi: GuestsService
  ) {}
  ngOnInit(){ this.load(); }

  load(){
    this.loading = true;
    this.api.list({ page:1, pageSize:100, q:this.q || undefined, status:this.status || undefined }).subscribe({
      next: d => {
        this.data.data = d;
        const uniqueBookingIds = Array.from(new Set(d.map(x => x.bookingId)));
        uniqueBookingIds.forEach(id => this.ensureBookingLoaded(id));
        this.loading = false;
      },
      error: _ => this.loading = false
    });
  }
  view(i: InvoiceListItem){ this.router.navigate(['/invoices', i.id]); }

  download(i: InvoiceListItem){
    this.api.downloadPdf(i.id).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `invoice_${i.id}.pdf`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  addInvoiceByRoom(){
    const roomNo = (this.roomNo || '').trim();
    if (!roomNo) return;
    this.loading = true;
    // 1) Find room by number via rooms search
    this.roomsApi.list(roomNo, undefined).subscribe({
      next: rooms => {
        const match = rooms.find(r => r.roomNumber.toLowerCase() === roomNo.toLowerCase()) || rooms[0];
        if (!match) { this.loading = false; alert('Room not found'); return; }
        // 2) Find latest CheckedOut booking for this room
        this.bookingsApi.list({ roomId: match.id, status: 'CheckedOut', page: 1, pageSize: 1 }).subscribe({
          next: bs => {
            if (!bs || bs.length === 0) { this.loading = false; alert('No checked-out booking found for this room'); return; }
            const booking = bs[0];
            // 3) Create invoice from booking
            this.api.createFromBooking(booking.id).subscribe({
              next: _ => { this.loading = false; this.load(); },
              error: err => { this.loading = false; alert(err?.error?.error || 'Failed to create invoice'); }
            });
          },
          error: _ => { this.loading = false; alert('Failed to search bookings'); }
        });
      },
      error: _ => { this.loading = false; alert('Failed to search rooms'); }
    });
  }

  private ensureBookingLoaded(bookingId:number){
    this.bookingsApi.get(bookingId).subscribe({ next: b => {
      this.ensureRoomLabelForBooking(bookingId, b.roomId);
      this.ensureGuestLabelForBooking(bookingId, b.guestId);
    }});
  }
  private ensureRoomLabelForBooking(bookingId:number, roomId:number){
    const apply = (label:string) => {
      const arr = this.data.data;
      for (const it of arr) if (it.bookingId === bookingId) it.roomNumber = label;
      this.data.data = arr;
    };
    if (this.roomCache.has(roomId)) { apply(this.roomCache.get(roomId)!); return; }
    this.roomsApi.get(roomId).subscribe({ next: r => { this.roomCache.set(roomId, r.roomNumber); apply(r.roomNumber); } });
  }
  private ensureGuestLabelForBooking(bookingId:number, guestId:number){
    const apply = (label:string) => {
      const arr = this.data.data;
      for (const it of arr) if (it.bookingId === bookingId) it.guestName = label;
      this.data.data = arr;
    };
    if (this.guestCache.has(guestId)) { apply(this.guestCache.get(guestId)!); return; }
    this.guestsApi.get(guestId).subscribe({ next: g => { this.guestCache.set(guestId, g.fullName); apply(g.fullName); } });
  }
}

// helpers
export interface BookingForLabels { roomId:number; guestId:number; }
