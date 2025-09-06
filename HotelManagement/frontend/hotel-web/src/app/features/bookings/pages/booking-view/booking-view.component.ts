import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingsService, BookingDetail } from '../../services/bookings.service';
import { RoomsService } from '../../../rooms/services/rooms.service';
import { GuestsService } from '../../../guests/services/guests.service';

@Component({
  selector: 'app-booking-view',
  templateUrl: './booking-view.component.html',
  styleUrls: ['./booking-view.component.scss']
})
export class BookingViewComponent implements OnInit {
  id!: number;
  b?: BookingDetail;
  loading = false; acting = false;
  roomLabel = '';
  guestLabel = '';

  constructor(
    private route: ActivatedRoute,
    private api: BookingsService,
    private router: Router,
    private roomsApi: RoomsService,
    private guestsApi: GuestsService
  ) {}

  ngOnInit(){
    this.id = +(this.route.snapshot.paramMap.get('id')!);
    this.load();
  }

  load(){
    this.loading = true;
    this.api.get(this.id).subscribe({ next: d => {
      this.b = d; this.loading=false;
      // fetch related labels
      this.roomsApi.get(d.roomId).subscribe({ next: r => this.roomLabel = r.roomNumber });
      this.guestsApi.get(d.guestId).subscribe({ next: g => this.guestLabel = g.fullName });
    }, error: _ => this.loading=false });
  }

  private toLocalDate(d: string | Date){
    return d instanceof Date ? new Date(d.getFullYear(), d.getMonth(), d.getDate()) : new Date(new Date(d).getFullYear(), new Date(d).getMonth(), new Date(d).getDate());
  }
  canConfirm(){ return !!this.b && this.b.status === 'Pending'; }
  // Enable by status only; server enforces the date window using the configured hotel timezone.
  canCheckin(){ return !!this.b && this.b.status === 'Confirmed'; }
  canCheckout(){ return !!this.b && this.b.status === 'CheckedIn'; }
  canCancel(){ return !!this.b && this.b.status !== 'Cancelled' && this.b.status !== 'CheckedOut'; }

  confirm(){
    this.acting=true;
    this.api.confirm(this.id).subscribe({ next: _ => {
      // On confirm, mark room occupied via RoomsService
      const roomId = this.b?.roomId;
      if (roomId) this.roomsApi.setStatus(roomId, { status: 'Occupied' }).subscribe({ next: ()=>{}, error: ()=>{} });
      this.acting=false; this.load();
    }, error: _ => this.acting=false });
  }
  checkin(){
    this.acting=true;
    this.api.checkin(this.id).subscribe({ next: _ => {
      const roomId = this.b?.roomId;
      if (roomId) this.roomsApi.setStatus(roomId, { status: 'Occupied' }).subscribe({ next: ()=>{}, error: ()=>{} });
      this.acting=false; this.load();
    }, error: _ => this.acting=false });
  }
  checkout(){
    this.acting=true;
    this.api.checkout(this.id).subscribe({ next: _ => {
      const roomId = this.b?.roomId;
      if (roomId) this.roomsApi.setStatus(roomId, { status: 'Available' }).subscribe({ next: ()=>{}, error: ()=>{} });
      this.acting=false; this.load();
    }, error: _ => this.acting=false });
  }
  cancel(){
    this.acting=true;
    this.api.cancel(this.id).subscribe({ next: _ => {
      const roomId = this.b?.roomId;
      if (roomId) this.roomsApi.setStatus(roomId, { status: 'Available' }).subscribe({ next: ()=>{}, error: ()=>{} });
      this.acting=false; this.load();
    }, error: _ => this.acting=false });
  }
  back(){ this.router.navigate(['/bookings']); }
  del(){
    if (!this.b || this.b.status !== 'Cancelled') return;
    if (!confirm(`Delete booking #${this.id}? This cannot be undone.`)) return;
    this.acting = true;
    this.api.delete(this.id).subscribe({
      next: _ => { this.acting = false; this.back(); },
      error: _ => { this.acting = false; }
    });
  }
}
