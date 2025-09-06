import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingsService, BookingCreateRequest } from '../../services/bookings.service';
import { RoomsService, Room } from '../../../rooms/services/rooms.service';
import { GuestsService, Guest } from '../../../guests/services/guests.service';

@Component({
  selector: 'app-booking-form',
  templateUrl: './booking-form.component.html',
  styleUrls: ['./booking-form.component.scss']
})
export class BookingFormComponent {
  rooms: Room[] = [];
  guests: Guest[] = [];
  useExistingGuest = true;

  // Form model
  roomId?: number;
  guestId?: number;
  newGuest: Partial<Guest> = { fullName: '', phone: '', email: '', idProof: '' };
  start?: Date; end?: Date;
  nightlyRate = 100;
  notes = '';
  saving = false;
  editId?: number;

  constructor(private api: BookingsService, private roomsApi: RoomsService, private guestsApi: GuestsService, private router: Router, private route: ActivatedRoute) {
    this.roomsApi.list('', 'Available').subscribe({ next: rs => this.rooms = rs });
    this.guestsApi.list(1, 100, '').subscribe({ next: gs => this.guests = gs });
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.editId = +idParam;
      // load booking to edit
      this.api.get(this.editId).subscribe({ next: b => {
        this.roomId = b.roomId;
        this.guestId = b.guestId;
        this.useExistingGuest = true;
        this.start = new Date(b.startDate);
        this.end = new Date(b.endDate);
        this.nightlyRate = b.nightlyRate ?? this.nightlyRate;
        this.notes = b.notes ?? '';
      }});
    }
  }

  private fmt(d?: Date){ if(!d) return ''; const y=d.getFullYear(); const m=String(d.getMonth()+1).padStart(2,'0'); const day=String(d.getDate()).padStart(2,'0'); return `${y}-${m}-${day}`; }

  save(){
    if (!this.roomId || !this.start || !this.end) return;
    if (this.end < this.start) { alert('End date must be after start date'); return; }

    const body: BookingCreateRequest = {
      roomId: this.roomId,
      startDate: this.fmt(this.start),
      endDate: this.fmt(this.end),
      nightlyRate: this.nightlyRate,
      notes: this.notes || undefined
    };

    if (this.useExistingGuest) {
      if (!this.guestId) { alert('Select a guest'); return; }
      body.guestId = this.guestId;
    } else {
      if (!this.newGuest.fullName) { alert('Enter guest name'); return; }
      body.guest = {
        fullName: this.newGuest.fullName!,
        phone: this.newGuest.phone ?? null,
        email: this.newGuest.email ?? null,
        idProof: this.newGuest.idProof ?? null
      };
    }

    this.saving = true;
    if (this.editId) {
      this.api.update(this.editId, body).subscribe({
        next: _ => { this.saving = false; this.router.navigate(['/bookings', this.editId]); },
        error: _ => this.saving = false
      });
    } else {
      this.api.create(body).subscribe({
        next: res => { this.saving = false; this.router.navigate(['/bookings', res.id]); },
        error: _ => this.saving = false
      });
    }
  }
}
