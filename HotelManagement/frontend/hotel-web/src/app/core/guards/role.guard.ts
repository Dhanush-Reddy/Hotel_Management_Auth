import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}
  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    const need = route.data?.['role'] as ('Admin'|'Staff'|undefined);
    if (!this.auth.isLoggedIn()) return this.router.parseUrl('/login');
    if (!need) return true;
    return this.auth.hasRole(need) ? true : this.router.parseUrl('/login');
  }
}
