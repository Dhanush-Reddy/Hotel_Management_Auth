import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export type BookingStatus = 'Pending'|'Confirmed'|'CheckedIn'|'CheckedOut'|'Cancelled';

export interface BookingListItem {
  id: number;
  roomId: number;
  roomNumber?: string;
  guestId: number;
  guestName?: string;
  status: BookingStatus;
  startDate: string; // ISO yyyy-MM-dd
  endDate: string;   // ISO yyyy-MM-dd
  nightlyRate: number;
}

export interface BookingDetail extends BookingListItem {
  notes?: string | null;
  createdAt?: string;
}

export interface BookingCreateRequest {
  roomId: number;
  guestId?: number;
  guest?: { fullName: string; phone?: string|null; email?: string|null; idProof?: string|null; };
  startDate: string; // yyyy-MM-dd
  endDate: string;   // yyyy-MM-dd
  nightlyRate: number;
  notes?: string | null;
}

@Injectable({ providedIn: 'root' })
export class BookingsService {
  private base = `${environment.apiBaseUrl}/api/bookings`;
  constructor(private http: HttpClient) {}

  list(opts: { page?:number; pageSize?:number; roomId?:number; guestId?:number; status?:BookingStatus; from?:string; to?:string } = {}) {
    let p = new HttpParams();
    if (opts.page) p = p.set('page', opts.page);
    if (opts.pageSize) p = p.set('pageSize', opts.pageSize);
    if (opts.roomId) p = p.set('roomId', opts.roomId);
    if (opts.guestId) p = p.set('guestId', opts.guestId);
    if (opts.status) p = p.set('status', opts.status);
    if (opts.from) p = p.set('from', opts.from);
    if (opts.to) p = p.set('to', opts.to);
    return this.http.get<BookingListItem[]>(this.base, { params: p });
  }

  get(id: number) { return this.http.get<BookingDetail>(`${this.base}/${id}`); }
  create(body: BookingCreateRequest) { return this.http.post<{id:number}>(this.base, body); }
  update(id: number, body: BookingCreateRequest) { return this.http.put<void>(`${this.base}/${id}`, body); }

  confirm(id: number) { return this.http.put<void>(`${this.base}/${id}/confirm`, {}); }
  checkin(id: number) { return this.http.put<void>(`${this.base}/${id}/checkin`, {}); }
  checkout(id: number) { return this.http.put<void>(`${this.base}/${id}/checkout`, {}); }
  cancel(id: number) { return this.http.put<void>(`${this.base}/${id}/cancel`, {}); }
  delete(id: number) { return this.http.delete<void>(`${this.base}/${id}`); }
}
