import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgreementViewComponent } from './agreement-view.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxsModule } from '@ngxs/store';
import { stub } from '@utils/stub-factory';
import { AuthService } from '@services/auth.service';
import { EventsTrackingService } from '@services/events-tracking.service';
import { DialogWindowService } from '@services/dialog-window.service';

describe('AgreementViewComponent', () => {
  let component: AgreementViewComponent;
  let fixture: ComponentFixture<AgreementViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TranslateModule.forRoot(), NgxsModule.forRoot()],
      providers: [stub(AuthService), stub(DialogWindowService), stub(EventsTrackingService)],
      declarations: [AgreementViewComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgreementViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
