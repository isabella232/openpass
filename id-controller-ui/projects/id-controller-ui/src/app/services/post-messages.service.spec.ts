import { TestBed } from '@angular/core/testing';

import { PostMessagesService } from './post-messages.service';
import { windowFactory } from '@utils/window-factory';

describe('PostMessagesService', () => {
  let service: PostMessagesService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: 'Window', useFactory: windowFactory }],
    });
    service = TestBed.inject(PostMessagesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
