import { Inject, Injectable } from '@angular/core';
import { Subscription } from 'rxjs';
import { CookiesService } from './cookies.service';
import { PublicApiService } from './public-api.service';
import { PostMessagesService } from './post-messages.service';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { environment } from '../../environments/environment';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MessageSubscriptionService {
  private messageSubscription: Subscription;

  constructor(
    @Inject('Window') private window: Window,
    private cookiesService: CookiesService,
    private postMessageService: PostMessagesService,
    private publicApiService: PublicApiService
  ) {}

  initTokenListener(openPassWindow: Window) {
    this.waitForPostMessage();
    this.postMessageService.startListening(openPassWindow);
  }

  destroyTokenListener() {
    this.postMessageService.stopListing();
    this.messageSubscription?.unsubscribe?.();
  }

  private waitForPostMessage() {
    this.messageSubscription = this.postMessageService
      .getSubscription()
      .pipe(filter(({ action }) => action === PostMessageActions.setToken))
      .subscribe((payload) => this.setCookie(payload));
  }

  private setCookie(payload: PostMessagePayload) {
    const { token, email, isDeclined } = payload;
    this.cookiesService.setCookie(environment.cookieName, token, 31);
    this.publicApiService.setUserData({ token, email, isDeclined });
  }
}
