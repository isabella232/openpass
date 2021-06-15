import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainViewComponent } from './main-view.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxsModule } from '@ngxs/store';
import { EventsTrackingService } from '@services/events-tracking.service';
import { stub } from '@utils/stub-factory';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { PostMessagesService } from '@services/post-messages.service';

describe('MainViewComponent', () => {
  let component: MainViewComponent;
  let fixture: ComponentFixture<MainViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TranslateModule.forRoot(), NgxsModule.forRoot()],
      declarations: [MainViewComponent],
      providers: [
        stub(EventsTrackingService, { trackEvent: () => {} }),
        stub(AuthService),
        stub(DialogWindowService),
        stub(PostMessagesService),
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MainViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
