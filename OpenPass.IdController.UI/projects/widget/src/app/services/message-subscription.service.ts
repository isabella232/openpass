import { Inject, Injectable } from '@angular/core';
import { Subscription } from 'rxjs';
import { CookiesService } from './cookies.service';
import { PublicApiService } from './public-api.service';
import { PostMessagesService } from './post-messages.service';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { environment } from '@env';
import { filter } from 'rxjs/operators';
import { WINDOW } from '@utils/injection-tokens';

@Injectable()
export class MessageSubscriptionService {
  private optOutSubscription: Subscription;
  private messageSubscription: Subscription;

  constructor(
    @Inject(WINDOW) private window: Window,
    private cookiesService: CookiesService,
    private postMessageService: PostMessagesService,
    private publicApiService: PublicApiService
  ) {}

  initTokenListener(openPassWindow: Window) {
    this.waitForOptOut();
    this.waitForPostMessage();
    this.postMessageService.startListening(openPassWindow);
  }

  destroyTokenListener() {
    this.postMessageService.stopListing();
    this.optOutSubscription?.unsubscribe?.();
    this.messageSubscription?.unsubscribe?.();
  }

  private waitForPostMessage() {
    this.messageSubscription = this.postMessageService
      .getSubscription()
      .pipe(filter(({ action }) => action === PostMessageActions.setToken))
      .subscribe((payload) => this.setCookie(payload));
  }

  private waitForOptOut() {
    this.optOutSubscription = this.postMessageService
      .getSubscription()
      .pipe(filter(({ action }) => action === PostMessageActions.optOut))
      .subscribe(() => {
        this.cookiesService.setCookie(environment.cookieUid2Token, '', -1);
        this.cookiesService.setCookie(environment.cookieIfaToken, '', -1);
        this.cookiesService.setCookie(environment.cookieOptoutFlag, '1', environment.cookieLifetimeDays);
      });
  }

  private setCookie(payload: PostMessagePayload) {
    const { ifaToken, uid2Token, isDeclined } = payload;
    this.cookiesService.setCookie(environment.cookieUid2Token, uid2Token, environment.cookieLifetimeDays);
    this.cookiesService.setCookie(environment.cookieIfaToken, ifaToken, environment.cookieLifetimeDays);
    this.cookiesService.setCookie(environment.cookieOptoutFlag, '', -1);
    this.publicApiService.setUserData({ ifaToken, uid2Token, isDeclined });
  }
}
