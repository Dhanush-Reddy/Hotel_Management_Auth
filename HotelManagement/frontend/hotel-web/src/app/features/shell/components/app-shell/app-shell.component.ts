import { Component } from '@angular/core';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-app-shell',
  templateUrl: './app-shell.component.html',
  styleUrls: ['./app-shell.component.scss']
})
export class AppShellComponent {
  constructor(private auth: AuthService) {}
  get isLoggedIn() { return this.auth.isLoggedIn(); }
  get username() { return this.auth.currentUser?.username ?? ''; }
  logout() { this.auth.logout(); }
}
