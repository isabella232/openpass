import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { NgxsModule } from '@ngxs/store';
import { PostMessagesService } from '@services/post-messages.service';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';

const storageName = 'USRF';
const updateStorageTokens = (value: { ifaToken?: string; uid2Token?: string; email?: string }) => {
  window.localStorage.setItem(storageName, JSON.stringify({ openpass: value }));
};

describe('AuthService', () => {
  let service: AuthService;
  let postMessagesService: { sendMessage: (message: PostMessagePayload) => void };

  beforeEach(() => {
    window.localStorage.removeItem(storageName);
    postMessagesService = { sendMessage: () => {} };
    TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot([])],
      providers: [{ provide: PostMessagesService, useValue: postMessagesService }],
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return ifa token', () => {
    const tokenValue = 'tokenValue';
    updateStorageTokens({ ifaToken: tokenValue });
    expect(service.ifaToken).toEqual(tokenValue);
  });

  it('should return uid2 token', () => {
    const tokenValue = 'UID2tokenValue';
    updateStorageTokens({ uid2Token: tokenValue });
    expect(service.uid2Token).toEqual(tokenValue);
  });

  it('should display using of an email', () => {
    const emailValue = 'fake@mail.com';
    updateStorageTokens({ email: emailValue });
    expect(service.isEmailUsed).toBeTruthy();
  });

  it('should send message to postMessagesService', () => {
    const postMessageServiceSpy = spyOn(postMessagesService, 'sendMessage');
    updateStorageTokens({ uid2Token: 'fake1', ifaToken: 'fake2' });
    service.setTokenToOpener();
    expect(postMessageServiceSpy).toHaveBeenCalledWith({
      action: PostMessageActions.setToken,
      ifaToken: 'fake2',
      uid2Token: 'fake1',
    });
  });

  it('should reset tokens', () => {
    updateStorageTokens({ uid2Token: 'fake1', ifaToken: 'fake2' });
    service.resetToken();
    expect(service.uid2Token).toBeFalsy();
    expect(service.ifaToken).toBeFalsy();
  });
});
