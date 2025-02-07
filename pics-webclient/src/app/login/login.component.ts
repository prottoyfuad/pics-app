import { Component, EventEmitter, Output } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { User } from '../models/user';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  // @Output() goToSignUpEvent = new EventEmitter<void>();
  constructor(private router: Router, 
              private authService: AuthService) {
    if (this.authService.getCurrentUser() !== null) { 
      this.router.navigateByUrl('home', { replaceUrl: true });
    }
  }

  user: User = new User();
  alert: string = "";

  login() {
    if (this.user.username.length === 0) {
      this.alert = "Username can not be empty!";
      return;
    }
    if (this.user.password.length === 0) {
      this.alert = "Password can not be empty!";
      return;
    }
    this.authService.login(this.user).subscribe({
      next: (data) => {
        this.authService.setCurrentUser(data),
        console.log("login: ", data);
      },
      error: (err) => {
        console.log(err);
        this.alert = `${err.statusText}: ${err.error}`;
      },
      complete: () => {
        console.log(`login done: ${this.user}`);
        this.goToPage('home');
      }
    });
  }

  goToPage(page: string) {
    this.router.navigateByUrl(page, { replaceUrl: true });
  }
}
