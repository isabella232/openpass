import { TestBed } from '@angular/core/testing';

import { GapiService } from './gapi.service';
import { WINDOW } from '@utils/injection-tokens';
import { environment } from '../../environments/environment.prod';

const noop = () => {};

type AuthInstance = {
  currentUser: {
    get: () => {
      getBasicProfile: () => {
        getEmail: () => string;
      };
    };
  };
  signOut: () => void;
  attachClickHandler: () => void;
};

type Gapi = {
  auth2: {
    getAuthInstance: () => AuthInstance;
    // eslint-disable-next-line @typescript-eslint/naming-convention
    init: (params: { client_id: string }) => any;
  };
  load: (type: string, callback: () => void) => any;
};

describe('GapiService', () => {
  let service: GapiService;
  let windowGapiMock: Gapi;
  let authInstance: AuthInstance;

  beforeEach(() => {
    authInstance = {
      currentUser: {
        get: () => ({
          getBasicProfile: () => ({
            getEmail: () => 'fake@mail.com',
          }),
        }),
      },
      signOut: noop,
      attachClickHandler: noop,
    };
    windowGapiMock = {
      auth2: {
        getAuthInstance: () => authInstance,
        init: noop,
      },
      load: noop,
    };
    TestBed.configureTestingModule({
      providers: [{ provide: WINDOW, useValue: { gapi: windowGapiMock } }],
    });
    service = TestBed.inject(GapiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return the email', () => {
    expect(service.userEmail).toEqual('fake@mail.com');
  });

  it('should init gapi services', async () => {
    windowGapiMock.load = (type: string, callback: () => void) => callback();
    const initSpy = spyOn(windowGapiMock.auth2, 'init');
    await service.load();
    // eslint-disable-next-line @typescript-eslint/naming-convention
    expect(initSpy).toHaveBeenCalledWith({ client_id: environment.googleClientId });
  });

  it('should call the "signOut" method in Google API', () => {
    const signOutSpy = spyOn(authInstance, 'signOut');
    service.signOut();
    expect(signOutSpy).toHaveBeenCalled();
  });

  it('should call "attachClickHandler" in Google API', async () => {
    windowGapiMock.load = (type: string, callback: () => void) => callback();
    const attachClickHandlerSpy = spyOn(authInstance, 'attachClickHandler');
    await service.load();
    await service.attachCustomButton(document.createElement('button'));
    expect(attachClickHandlerSpy).toHaveBeenCalled();
  });
});
