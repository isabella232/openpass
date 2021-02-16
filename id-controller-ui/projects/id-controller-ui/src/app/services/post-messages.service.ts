import { Inject, Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { PostMessageData } from '@shared/types/post-message-data';
import { PostMessageTypes } from '@shared/enums/post-message-types.enum';
import { PostMessagePayload } from '@shared/types/post-message-payload';

@Injectable({
  providedIn: 'root',
})
export class PostMessagesService {
  private trustedOrigin: string;
  private originMessage = new Subject();

  constructor(@Inject('Window') private window: Window) {}

  setTrustedSite(value: string) {
    this.trustedOrigin = value;
  }

  startListing(trustedOrigin: string) {
    this.setTrustedSite(trustedOrigin);

    this.window.addEventListener('message', (event) => {
      if (event.origin !== this.trustedOrigin) {
        return;
      }
      this.originMessage.next(event.data);
    });
  }

  getSubscription() {
    return this.originMessage.asObservable();
  }

  sendMessage(payload: PostMessagePayload) {
    const data: PostMessageData = {
      type: PostMessageTypes.openPass,
      payload,
    };
    (this.window.opener || this.window.parent).postMessage(data, this.trustedOrigin);
  }
}
