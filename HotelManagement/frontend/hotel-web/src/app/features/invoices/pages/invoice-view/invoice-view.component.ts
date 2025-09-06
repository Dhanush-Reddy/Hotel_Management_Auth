import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InvoicesService, InvoiceDetail } from '../../services/invoices.service';
import { BookingsService } from '../../../bookings/services/bookings.service';
import { RoomsService } from '../../../rooms/services/rooms.service';
import { GuestsService } from '../../../guests/services/guests.service';

@Component({
  selector: 'app-invoice-view',
  templateUrl: './invoice-view.component.html',
  styleUrls: ['./invoice-view.component.scss']
})
export class InvoiceViewComponent implements OnInit {
  id!: number;
  inv?: InvoiceDetail;
  loading = false;
  downloading = false;
  paying = false;

  constructor(
    private route: ActivatedRoute,
    private api: InvoicesService,
    private bookingsApi: BookingsService,
    private roomsApi: RoomsService,
    private guestsApi: GuestsService
  ) {}

  ngOnInit(){
    this.id = +(this.route.snapshot.paramMap.get('id')!);
    this.load();
  }

  load(){
    this.loading = true;
    this.api.get(this.id).subscribe({
      next: d => { this.inv = d; this.loading = false; this.loadLabels(); },
      error: _ => this.loading = false
    });
  }

  private loadLabels(){
    if (!this.inv) return;
    this.bookingsApi.get(this.inv.bookingId).subscribe({ next: b => {
      this.roomsApi.get(b.roomId).subscribe({ next: r => { if (this.inv) this.inv.roomNumber = r.roomNumber; } });
      this.guestsApi.get(b.guestId).subscribe({ next: g => { if (this.inv) this.inv.guestName = g.fullName; } });
    }});
  }

  download(){
    this.downloading = true;
    this.api.downloadPdf(this.id).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url; a.download = `invoice_${this.id}.pdf`; a.click();
        URL.revokeObjectURL(url);
        this.downloading = false;
      },
      error: _ => this.downloading = false
    });
  }

  pay(){
    if (!this.inv || this.inv.status === 'Paid') return;
    this.paying = true;
    this.api.markPaid(this.id).subscribe({
      next: _ => { if (this.inv) this.inv.status = 'Paid'; this.paying = false; },
      error: _ => { this.paying = false; }
    });
  }
}
