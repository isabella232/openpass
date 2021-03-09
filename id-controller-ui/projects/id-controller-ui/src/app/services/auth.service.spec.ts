import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { windowFactory } from '@utils/window-factory';
import { NgxsModule } from '@ngxs/store';
import { WINDOW } from '@utils/injection-tokens';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot([])],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
