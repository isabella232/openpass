import { TestBed } from '@angular/core/testing';

import { RecognizedGuard } from './recognized.guard';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxsModule } from '@ngxs/store';
import { windowFactory } from '@utils/window-factory';
import { WINDOW } from '@utils/injection-tokens';

describe('RecognizedGuard', () => {
  let guard: RecognizedGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, NgxsModule.forRoot([])],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
    });
    guard = TestBed.inject(RecognizedGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
