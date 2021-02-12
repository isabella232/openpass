import { Injectable } from '@angular/core';
import { CookiesService } from '@services/cookies.service';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PostMessagesService } from '@services/post-messages.service';
import { environment } from '@env';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  cookieName = environment.cookieName; // TODO change cookie name

  get isAuthenticated(): boolean {
    return !!this.token;
  }

  get token(): string {
    return this.cookiesService.getCookie(this.cookieName);
  }

  constructor(private cookiesService: CookiesService, private postMessagesService: PostMessagesService) {}

  setTokenToOpener() {
    const message: PostMessagePayload = {
      action: PostMessageActions.setToken,
      token: this.token,
    };
    this.postMessagesService.sendMessage(message);
  }
}
