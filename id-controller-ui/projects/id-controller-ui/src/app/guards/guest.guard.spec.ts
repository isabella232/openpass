import { TestBed } from '@angular/core/testing';

import { GuestGuard } from './guest.guard';
import { windowFactory } from '@utils/window-factory';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxsModule } from '@ngxs/store';

describe('GuestGuard', () => {
  let guard: GuestGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, NgxsModule.forRoot([])],
      providers: [{ provide: 'Window', useFactory: windowFactory }],
    });
    guard = TestBed.inject(GuestGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
