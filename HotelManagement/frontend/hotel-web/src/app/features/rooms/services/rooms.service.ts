import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export type Room = {
  id: number;
  roomNumber: string;
  capacity: number;
  status: 'Available'|'Occupied'|'OutOfService';
  createdAt?: string;
};
export type CreateRoom = { roomNumber: string; capacity: number; };
export type UpdateRoom = { roomNumber: string; capacity: number; };
export type SetStatus  = { status: 'Available'|'Occupied'|'OutOfService' };

@Injectable({ providedIn: 'root' })
export class RoomsService {
  private base = `${environment.apiBaseUrl}/api/rooms`;
  constructor(private http: HttpClient) {}

  list(q?: string, status?: string) {
    let params = new HttpParams();
    if (q) params = params.set('q', q);
    if (status) params = params.set('status', status);
    return this.http.get<Room[]>(this.base, { params });
  }
  get(id: number) { return this.http.get<Room>(`${this.base}/${id}`); }
  create(body: CreateRoom) { return this.http.post<{id:number}>(this.base, body); }
  update(id: number, body: UpdateRoom) { return this.http.put<void>(`${this.base}/${id}`, body); }
  remove(id: number) { return this.http.delete<void>(`${this.base}/${id}`); }
  setStatus(id: number, body: SetStatus) { return this.http.put<void>(`${this.base}/${id}/status`, body); }
}
