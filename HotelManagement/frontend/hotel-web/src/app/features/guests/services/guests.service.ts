import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export type Guest = {
  id:number; fullName:string; phone?:string|null; email?:string|null; idProof?:string|null; createdAt?:string;
};
export type CreateGuest = { fullName:string; phone?:string|null; email?:string|null; idProof?:string|null; };
export type UpdateGuest = Partial<CreateGuest>;

@Injectable({ providedIn:'root' })
export class GuestsService {
  private base = `${environment.apiBaseUrl}/api/guests`;
  constructor(private http: HttpClient) {}

  list(page=1, pageSize=50, q?:string){
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (q) params = params.set('q', q);
    return this.http.get<Guest[]>(this.base, { params });
  }
  search(q: string){
    const url = `${this.base}`;
    const params = q ? new HttpParams().set('q', q) : undefined as any;
    return this.http.get<Guest[]>(url, { params });
  }
  get(id:number){ return this.http.get<Guest>(`${this.base}/${id}`); }
  create(body:CreateGuest){ return this.http.post<{id:number}>(this.base, body); }
  update(id:number, body:UpdateGuest){ return this.http.put<void>(`${this.base}/${id}`, body); }
  remove(id:number){ return this.http.delete<void>(`${this.base}/${id}`); }
}
