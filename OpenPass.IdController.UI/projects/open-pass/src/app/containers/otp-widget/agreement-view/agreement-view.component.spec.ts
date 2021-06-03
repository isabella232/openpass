import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgreementViewComponent } from './agreement-view.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxsModule } from '@ngxs/store';
import { stub } from '@utils/stub-factory';
import { AuthService } from '@services/auth.service';
import { EventsTrackingService } from '@services/events-tracking.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'usrf-copyright',
  template: '',
})
class StubCopyrightComponent {}

describe('AgreementViewComponent', () => {
  let component: AgreementViewComponent;
  let fixture: ComponentFixture<AgreementViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TranslateModule.forRoot(), NgxsModule.forRoot(), FormsModule],
      providers: [stub(AuthService), stub(DialogWindowService), stub(EventsTrackingService)],
      declarations: [AgreementViewComponent, StubCopyrightComponent],
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
