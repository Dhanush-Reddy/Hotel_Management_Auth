import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) {}
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err: HttpErrorResponse) => {
        const isLogin = req.url.includes('/api/auth/login');
        if (err.status === 401 && !isLogin) this.auth.logout();
        if (!isLogin) {
          const message = (err.error?.message as string) ?? `Error ${err.status}`;
          alert(message);
        }
        return throwError(() => err);
      })
    );
  }
}
