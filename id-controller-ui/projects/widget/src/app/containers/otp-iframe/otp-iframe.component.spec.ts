import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtpIframeComponent } from './otp-iframe.component';
import { windowFactory } from '../../utils/window-factory';

describe('OtpIframeComponent', () => {
  let component: OtpIframeComponent;
  let fixture: ComponentFixture<OtpIframeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OtpIframeComponent],
      providers: [{ provide: 'Window', useFactory: windowFactory }],
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
