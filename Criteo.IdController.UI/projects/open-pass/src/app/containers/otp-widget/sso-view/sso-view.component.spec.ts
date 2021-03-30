import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SsoViewComponent } from './sso-view.component';
import { AuthenticatedService } from '@rest/authenticated/authenticated.service';
import { NgxsModule } from '@ngxs/store';
import { EventsTrackingService } from '@services/events-tracking.service';
import { stub } from '@utils/stub-factory';
import { PostMessagesService } from '@services/post-messages.service';
import { DialogWindowService } from '@services/dialog-window.service';

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
      declarations: [SsoViewComponent],
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
