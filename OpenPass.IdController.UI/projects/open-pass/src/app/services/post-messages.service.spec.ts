import { TestBed } from '@angular/core/testing';

import { PostMessagesService } from './post-messages.service';
import { NgxsModule, Store } from '@ngxs/store';
import { WINDOW } from '@utils/injection-tokens';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageTypes } from '@shared/enums/post-message-types.enum';

describe('PostMessagesService', () => {
  let service: PostMessagesService;
  let windowMock: Partial<Window>;
  let storeMock: Partial<Store>;
  const fakeOrigin = 'fake_origin';

  beforeEach(() => {
    windowMock = {
      addEventListener: () => {},
      opener: {
        postMessage: () => {},
      },
      parent: null,
    };
    storeMock = {
      selectSnapshot: () => fakeOrigin,
    };
    TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot([])],
      providers: [
        { provide: WINDOW, useValue: windowMock },
        { provide: Store, useValue: storeMock },
      ],
    });
    service = TestBed.inject(PostMessagesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should subscribe to the "message" event', () => {
    const listenerSpy = spyOn(windowMock, 'addEventListener');
    service.startListening();
    expect(listenerSpy).toHaveBeenCalled();
  });

  it('should send the message to origin', () => {
    const originSpy = spyOn(windowMock.opener, 'postMessage');
    const payload: PostMessagePayload = { action: PostMessageActions.closeChild };
    service.sendMessage(payload);
    expect(originSpy).toHaveBeenCalledWith(
      {
        type: PostMessageTypes.openPass,
        payload,
      },
      fakeOrigin
    );
  });
});
