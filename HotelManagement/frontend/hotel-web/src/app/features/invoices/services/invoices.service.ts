import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { map } from 'rxjs/operators';

export type InvoiceStatus = 'Unpaid'|'Paid';
export interface InvoiceListItem {
  id:number;
  bookingId:number;
  guestName:string;
  roomNumber:string;
  totalAmount:number;
  status:InvoiceStatus;
  issuedAt:string;   // ISO
}
export interface InvoiceDetail extends InvoiceListItem {
  nightlyRate:number;
  nights:number;
}

@Injectable({ providedIn: 'root' })
export class InvoicesService {
  private base = `${environment.apiBaseUrl}/api/invoices`;
  constructor(private http: HttpClient) {}

  private mapList(api: any[]): InvoiceListItem[] {
    return (api || []).map(x => ({
      id: x.id,
      bookingId: x.bookingId,
      guestName: '',
      roomNumber: '',
      totalAmount: x.subtotal,
      status: x.status,
      issuedAt: x.createdAt
    }));
  }

  private mapDetail(x: any): InvoiceDetail {
    return {
      id: x.id,
      bookingId: x.bookingId,
      guestName: '',
      roomNumber: '',
      totalAmount: x.subtotal,
      status: x.status,
      issuedAt: x.createdAt,
      nightlyRate: x.nightlyRate,
      nights: x.nights
    };
  }

  list(opts: { page?:number; pageSize?:number; q?:string; status?:InvoiceStatus } = {}) {
    let p = new HttpParams();
    if (opts.page) p = p.set('page', opts.page);
    if (opts.pageSize) p = p.set('pageSize', opts.pageSize);
    if (opts.q) p = p.set('q', opts.q);
    if (opts.status) p = p.set('status', opts.status);
    return this.http.get<any[]>(this.base, { params: p }).pipe(map(arr => this.mapList(arr)));
  }

  get(id:number) {
    return this.http.get<any>(`${this.base}/${id}`).pipe(map(x => this.mapDetail(x)));
  }

  downloadPdf(id:number) {
    const url = `${this.base}/${id}/pdf`;
    return this.http.get(url, { responseType: 'blob' as const });
  }

  createFromBooking(bookingId:number){
    return this.http.post<{id:number}>(`${this.base}/from-booking/${bookingId}`, {});
  }

  markPaid(id:number){
    return this.http.put<void>(`${this.base}/${id}/pay`, {});
  }
}
