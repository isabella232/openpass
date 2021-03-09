import { TestBed } from '@angular/core/testing';

import { PostMessagesService } from './post-messages.service';
import { windowFactory } from '@utils/window-factory';
import { NgxsModule } from '@ngxs/store';
import { WINDOW } from '@utils/injection-tokens';

describe('PostMessagesService', () => {
  let service: PostMessagesService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot([])],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
    });
    service = TestBed.inject(PostMessagesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
