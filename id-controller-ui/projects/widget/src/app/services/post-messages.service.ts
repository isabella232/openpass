import { Inject, Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { environment } from '../../environments/environment';
import { PostMessageData } from '@shared/types/post-message-data';
import { PostMessagePayload } from '@shared/types/post-message-payload';

@Injectable({
  providedIn: 'root',
})
export class PostMessagesService {
  private readonly messageType = 'open-pass';

  private trustedOrigin = new URL(environment.idControllerAppUrl).origin;
  private originMessage = new Subject<PostMessagePayload>();
  private receiverWindow: Window;

  constructor(@Inject('Window') private window: Window) {
    this.listener = this.listener.bind(this);
  }

  getSubscription() {
    return this.originMessage.asObservable();
  }

  startListing(window: Window) {
    this.setReceiverWindow(window);

    this.window.addEventListener('message', this.listener);
  }

  stopListing() {
    this.window.removeEventListener('message', this.listener);
  }

  private listener(event: MessageEvent<PostMessageData>) {
    if (event.origin !== this.trustedOrigin || event.data?.type !== this.messageType) {
      return;
    }
    this.originMessage.next(event.data.payload);
  }

  private setReceiverWindow(window: Window) {
    this.receiverWindow = window;
  }
}
