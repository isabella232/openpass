import { Injectable } from '@angular/core';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PostMessagesService } from '@services/post-messages.service';
import { localStorage } from '@shared/utils/storage-decorator';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  @localStorage('openpass.ifaToken')
  private storageIfaToken: string;
  @localStorage('openpass.uid2Token')
  private storageUid2Token: string;
  @localStorage('openpass.email')
  private storageUserEmail: string;

  get uid2Token(): string {
    return this.storageUid2Token;
  }

  get ifaToken(): string {
    return this.storageIfaToken;
  }

  get isEmailUsed(): boolean {
    return !!this.storageUserEmail;
  }

  constructor(private postMessagesService: PostMessagesService) {}

  setTokenToOpener() {
    const message: PostMessagePayload = {
      action: PostMessageActions.setToken,
      ifaToken: this.storageIfaToken,
      uid2Token: this.storageUid2Token,
    };
    this.postMessagesService.sendMessage(message);
  }

  resetToken() {
    this.storageIfaToken = null;
    this.storageUid2Token = null;
  }
}
