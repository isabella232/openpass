import { TestBed } from '@angular/core/testing';

import { GapiService } from './gapi.service';
import { WINDOW } from '@utils/injection-tokens';
import { windowFactory } from '@utils/window-factory';

describe('GapiService', () => {
  let service: GapiService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
    });
    service = TestBed.inject(GapiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
