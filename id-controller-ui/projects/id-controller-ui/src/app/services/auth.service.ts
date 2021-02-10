import { Injectable } from '@angular/core';
import { CookiesService } from '@services/cookies.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  cookieName = 'OP_token'; // TODO change cookie name

  get isAuthenticated(): boolean {
    return !!this.token;
  }

  get token(): string {
    return this.cookiesService.getCookie(this.cookieName);
  }

  constructor(private cookiesService: CookiesService) {}
}
