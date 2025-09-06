import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

type LoginRequest = { username: string; password: string; };
type LoginResponse = { token: string; };
type MeResponse = { id: number; username: string; role: 'Admin'|'Staff'; };

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'hotel.jwt';
  public currentUser: MeResponse | null = null;

  constructor(private http: HttpClient, private router: Router) {
    const token = this.getToken();
    if (token) this.me().subscribe({ next: () => {}, error: () => this.logout() });
  }

  login(req: LoginRequest) {
    return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/api/auth/login`, req);
  }

  me() {
    return this.http.get<MeResponse>(`${environment.apiBaseUrl}/api/auth/me`);
  }

  afterLogin(token: string) {
    this.storeToken(token);
    this.me().subscribe({ next: _ => this.router.navigate(['/rooms']), error: _ => this.logout() });
  }

  storeToken(token: string) { localStorage.setItem(this.tokenKey, token); }
  getToken() { return localStorage.getItem(this.tokenKey); }
  isLoggedIn() { return !!this.getToken(); }
  hasRole(role: 'Admin'|'Staff') {
    const r = this.currentUser?.role;
    return r === role || (role === 'Staff' && r === 'Admin');
  }
  logout() { localStorage.removeItem(this.tokenKey); this.currentUser = null; this.router.navigate(['/login']); }
}

