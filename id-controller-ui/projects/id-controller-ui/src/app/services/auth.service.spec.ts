import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { windowFactory } from '@utils/window-factory';
import { NgxsModule } from '@ngxs/store';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot([])],
      providers: [{ provide: 'Window', useFactory: windowFactory }],
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
