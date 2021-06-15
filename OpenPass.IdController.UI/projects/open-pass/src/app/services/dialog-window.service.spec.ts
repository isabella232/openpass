import { TestBed } from '@angular/core/testing';

import { DialogWindowService } from './dialog-window.service';
import { WINDOW } from '@utils/injection-tokens';
import { NgxsModule, Store } from '@ngxs/store';
import { AuthService } from '@services/auth.service';
import { PostMessagesService } from '@services/post-messages.service';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PostMessagePayload } from '@shared/types/post-message-payload';

describe('DialogWindowService', () => {
  let service: DialogWindowService;
  const postMessagesServiceMock = { sendMessage: (payload: PostMessagePayload) => {} };
  const storeMock = { selectSnapshot: () => {} };
  const authServiceMock = {
    ifaToken: '',
    uid2Token: '',
  };
  const windowMock = {
    close: () => {},
    location: {
      replace: (url: string) => {},
    },
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot()],
      providers: [
        { provide: WINDOW, useValue: windowMock },
        { provide: Store, useValue: storeMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: PostMessagesService, useValue: postMessagesServiceMock },
      ],
    });
    service = TestBed.inject(DialogWindowService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send a message while closing the window', () => {
    const sendMessageSpy = spyOn(postMessagesServiceMock, 'sendMessage');
    const closeSpy = spyOn(windowMock, 'close');
    service.closeDialogWindow(true);
    expect(sendMessageSpy).toHaveBeenCalledWith({
      action: PostMessageActions.closeChild,
      isDeclined: true,
    });
    expect(closeSpy).toHaveBeenCalled();

    service.closeDialogWindow();
    expect(sendMessageSpy).toHaveBeenCalledWith({
      action: PostMessageActions.closeChild,
      isDeclined: false,
    });
  });

  it('should proceed to origin website (redirect mode)', () => {
    const origin = 'http://localhost';
    const locationReplaceSpy = spyOn(windowMock.location, 'replace');
    authServiceMock.ifaToken = 'ifaToken';
    authServiceMock.uid2Token = 'uid2Token';
    storeMock.selectSnapshot = () => origin;
    service.closeDialogWindow();
    expect(locationReplaceSpy).toHaveBeenCalledWith(`${origin}/?ifaToken=ifaToken&uid2Token=uid2Token`);
  });
});
