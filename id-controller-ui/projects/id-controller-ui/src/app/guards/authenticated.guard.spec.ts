import { TestBed } from '@angular/core/testing';

import { AuthenticatedGuard } from './authenticated.guard';
import { windowFactory } from '@utils/window-factory';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxsModule } from '@ngxs/store';
import { WINDOW } from '@utils/injection-tokens';

describe('AuthenticatedGuard', () => {
  let guard: AuthenticatedGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, NgxsModule.forRoot([])],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
    });
    guard = TestBed.inject(AuthenticatedGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
