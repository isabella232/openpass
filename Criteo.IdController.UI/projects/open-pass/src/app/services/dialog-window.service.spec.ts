import { TestBed } from '@angular/core/testing';

import { DialogWindowService } from './dialog-window.service';
import { WINDOW } from '@utils/injection-tokens';
import { windowFactory } from '@utils/window-factory';
import { NgxsModule } from '@ngxs/store';

describe('DialogWindowService', () => {
  let service: DialogWindowService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot()],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
    });
    service = TestBed.inject(DialogWindowService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
