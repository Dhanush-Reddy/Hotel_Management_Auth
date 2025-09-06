import { Component } from '@angular/core';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  username = '';
  password = '';
  loading = false;
  loginError = false;

  constructor(private auth: AuthService) {}

  submit() {
    if (!this.username || !this.password) return;
    this.loading = true;
    this.loginError = false;
    this.auth.login({ username: this.username, password: this.password }).subscribe({
      next: res => { this.loading = false; this.auth.afterLogin(res.token); },
      error: _ => { this.loading = false; this.loginError = true; }
    });
  }
}
