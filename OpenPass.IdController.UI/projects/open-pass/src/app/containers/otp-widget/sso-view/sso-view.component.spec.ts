import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SsoViewComponent } from './sso-view.component';
import { AuthenticatedService } from '@rest/authenticated/authenticated.service';
import { NgxsModule } from '@ngxs/store';
import { EventsTrackingService } from '@services/events-tracking.service';
import { stub } from '@utils/stub-factory';
import { PostMessagesService } from '@services/post-messages.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { Component } from '@angular/core';

@Component({
  selector: 'usrf-facebook-auth-button',
  template: '',
})
class StubFacebookButtonComponent {}

@Component({
  selector: 'usrf-google-auth-button',
  template: '',
})
class StubGoogleButtonComponent {}

@Component({
  selector: 'usrf-or-hr',
  template: '',
})
class StubOrComponent {}

describe('SsoViewComponent', () => {
  let component: SsoViewComponent;
  let fixture: ComponentFixture<SsoViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot()],
      providers: [
        stub(PostMessagesService),
        stub(DialogWindowService),
        stub(AuthenticatedService),
        stub(EventsTrackingService),
      ],
      declarations: [SsoViewComponent, StubGoogleButtonComponent, StubFacebookButtonComponent, StubOrComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SsoViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
