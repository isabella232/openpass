import { Injectable } from '@angular/core';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PostMessagesService } from '@services/post-messages.service';
import { localStorage } from '@shared/utils/storage-decorator';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  @localStorage('openpass.token')
  private storageUserToken: string;
  @localStorage('openpass.email')
  private storageUserEmail: string;

  get isAuthenticated(): boolean {
    return !!this.storageUserToken;
  }

  get isEmailUsed(): boolean {
    return !!this.storageUserEmail;
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
