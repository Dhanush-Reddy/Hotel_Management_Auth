import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { BookingsService, BookingListItem, BookingStatus } from '../../services/bookings.service';
import { RoomsService } from '../../../rooms/services/rooms.service';
import { GuestsService } from '../../../guests/services/guests.service';

@Component({
  selector: 'app-bookings-list',
  templateUrl: './bookings-list.component.html',
  styleUrls: ['./bookings-list.component.scss']
})
export class BookingsListComponent implements OnInit {
  data = new MatTableDataSource<BookingListItem>([]);
  cols = ['id','roomNumber','guestName','dates','status','rate','actions'];
  status: ''|BookingStatus = '';
  from?: Date; to?: Date;
  loading = false;

  private roomCache = new Map<number, string>();
  private guestCache = new Map<number, string>();

  constructor(
    private api: BookingsService,
    private router: Router,
    private roomsApi: RoomsService,
    private guestsApi: GuestsService
  ) {}
  ngOnInit(){ this.load(); }

  private fmt(d?: Date){ if(!d) return undefined; const y=d.getFullYear(); const m=String(d.getMonth()+1).padStart(2,'0'); const day=String(d.getDate()).padStart(2,'0'); return `${y}-${m}-${day}`; }

  load(){
    this.loading = true;
    this.api.list({
      status: this.status || undefined,
      from: this.fmt(this.from),
      to: this.fmt(this.to),
      page: 1, pageSize: 100
    }).subscribe({
      next: d => {
        // sort ascending by id
        d.sort((a,b)=>a.id-b.id);
        // hide cancelled by default unless explicitly filtered
        const filtered = (this.status && this.status.length > 0) ? d : d.filter(x => x.status !== 'Cancelled');
        this.data.data = filtered;
        // enrich with room number and guest name (backend returns only ids)
        const uniqueRoomIds = Array.from(new Set(this.data.data.map(x => x.roomId)));
        const uniqueGuestIds = Array.from(new Set(this.data.data.map(x => x.guestId)));
        uniqueRoomIds.forEach(id => this.ensureRoomLoaded(id));
        uniqueGuestIds.forEach(id => this.ensureGuestLoaded(id));
        this.loading = false;
      },
      error: _ => this.loading = false
    });
  }
  private ensureRoomLoaded(id:number){
    if (this.roomCache.has(id)) {
      this.applyRoomLabel(id, this.roomCache.get(id)!);
      return;
    }
    this.roomsApi.get(id).subscribe({ next: r => { this.roomCache.set(id, r.roomNumber); this.applyRoomLabel(id, r.roomNumber); } });
  }
  private applyRoomLabel(id:number, label:string){
    const arr = this.data.data;
    for (const it of arr) if (it.roomId === id) (it as any).roomNumber = label;
    this.data.data = arr; // trigger table update
  }
  private ensureGuestLoaded(id:number){
    if (this.guestCache.has(id)) {
      this.applyGuestLabel(id, this.guestCache.get(id)!);
      return;
    }
    this.guestsApi.get(id).subscribe({ next: g => { const name = g.fullName; this.guestCache.set(id, name); this.applyGuestLabel(id, name); } });
  }
  private applyGuestLabel(id:number, label:string){
    const arr = this.data.data;
    for (const it of arr) if (it.guestId === id) (it as any).guestName = label;
    this.data.data = arr;
  }

  view(b: BookingListItem){ this.router.navigate(['/bookings', b.id]); }
  edit(b: BookingListItem){ this.router.navigate(['/bookings', b.id, 'edit']); }
  new(){ this.router.navigate(['/bookings/new']); }
  del(b: BookingListItem){
    if (!confirm(`Delete booking #${b.id}? This cannot be undone.`)) return;
    this.api.delete(b.id).subscribe({ next: _ => this.load() });
  }
}
