import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { windowFactory } from '@utils/window-factory';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: 'Window', useFactory: windowFactory }],
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
