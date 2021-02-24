import { Injectable } from '@angular/core';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PostMessagesService } from '@services/post-messages.service';
import { environment } from '@env';
import { localStorage } from '@utils/storage-decorator';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  @localStorage('openpass.token')
  private storageUserToken: string;

  cookieName = environment.cookieName;

  get isAuthenticated(): boolean {
    return !!this.storageUserToken;
  }

  constructor(private postMessagesService: PostMessagesService) {}

  setTokenToOpener() {
    const message: PostMessagePayload = {
      action: PostMessageActions.setToken,
      token: this.storageUserToken,
    };
    this.postMessagesService.sendMessage(message);
  }

  resetToken() {
    this.storageUserToken = null;
  }
}
