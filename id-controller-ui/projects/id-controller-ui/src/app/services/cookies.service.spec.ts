import { TestBed } from '@angular/core/testing';

import { CookiesService } from './cookies.service';
import { windowFactory } from '@utils/window-factory';

describe('CookiesService', () => {
  let service: CookiesService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: 'Window', useFactory: windowFactory }],
    });
    service = TestBed.inject(CookiesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
