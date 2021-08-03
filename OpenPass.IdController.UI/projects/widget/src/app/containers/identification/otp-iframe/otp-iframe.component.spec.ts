import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtpIframeComponent } from './otp-iframe.component';
import { windowFactory } from '@utils/window-factory';
import { WINDOW } from '@utils/injection-tokens';
import { MessageSubscriptionService } from '@services/message-subscription.service';

describe('OtpIframeComponent', () => {
  let component: OtpIframeComponent;
  let fixture: ComponentFixture<OtpIframeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OtpIframeComponent],
      providers: [
        { provide: WINDOW, useFactory: windowFactory },
        {
          provide: MessageSubscriptionService,
          useValue: {
            initTokenListener: () => {},
            destroyTokenListener: () => {},
          },
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OtpIframeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
