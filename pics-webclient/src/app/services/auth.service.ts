import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { User } from '../models/user';
import { UserJwt } from '../models/userJwt';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUser: string | null = null;
  private apiUrl = 'http://localhost:5272/auth'

  constructor(private http: HttpClient,
              private cookieService: CookieService) {
    console.log("Auth Service Initialized");
    let jwt: UserJwt = new UserJwt(
      this.cookieService.get("username"),
      this.cookieService.get("jwt")
    );
    if (jwt.token.length > 0 && jwt.username.length > 0) {
      this.setCurrentUser(jwt);
    } else {
      this.setCurrentUser(null);
    }
  }
  
  getCurrentUser() {
    return this.currentUser;
  }

  setCurrentUser(user : UserJwt | null) {
    if (user === null) {
      this.currentUser = null;
      this.cookieService.delete("username");
      this.cookieService.delete("jwt");
    } else {
      this.currentUser = user.username;
      this.cookieService.set("username", user.username);
      this.cookieService.set("jwt", user.token);
    }
  }

  login(user: User) {
    return this.http.post<UserJwt>(`${this.apiUrl}/login`, user);
  }
  
  signup(user: User) {
    return this.http.post<UserJwt>(`${this.apiUrl}/register`, user);
  }
}
