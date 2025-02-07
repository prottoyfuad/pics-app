import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { User } from '../models/user';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [FormsModule, FormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  constructor(private router: Router,
              private authService: AuthService) {
    if (this.authService.getCurrentUser() !== null) { 
      this.router.navigateByUrl('home', { replaceUrl: true });
    }
  }
  // @Output() goToLoginEvent = new EventEmitter<void>();
  user: User = new User();
  alert: string = "";
  confirmedPassword: string = "";

  signup() {
    if (this.user.username.length === 0) {
      this.alert = "Username can not be empty!";
      return;
    }
    if (this.user.password.length === 0 || this.confirmedPassword.length === 0) {
      this.alert = "Password can not be empty!";
      return;
    }
    if (this.user.password !== this.confirmedPassword) {
      this.alert = "Passwords do not match!";
      return;
    }
    this.authService.signup(this.user).subscribe({
      next: (data) => {
        this.authService.setCurrentUser(data);
        console.log(data);
        this.goToPage('home');
      },
      error: (err) => {
        console.log(err);
        this.alert = `${err.statusText} ${err.error}`;
      },
      complete: () => console.log(`signup done: ${this.user}`)
    });
  }

  goToPage(page: string) {
    this.router.navigateByUrl(page, { replaceUrl: true });
  }
}
