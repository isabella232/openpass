import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainViewComponent } from './main-view.component';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxsModule } from '@ngxs/store';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { stub } from '@utils/stub-factory';
import { EventsService } from '@rest/events/events.service';
import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';

@Component({
  selector: 'usrf-open-pass-details',
  template: '',
})
class StubOpenPassDetailsComponent {
  @Input() name: string;
}

describe('MainViewComponent', () => {
  let component: MainViewComponent;
  let fixture: ComponentFixture<MainViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, NgxsModule.forRoot(), TranslateModule.forRoot(), FormsModule],
      providers: [
        stub(AuthService),
        stub(DialogWindowService),
        stub(EventsService, { trackEvent: () => new Observable() }),
      ],
      declarations: [MainViewComponent, StubOpenPassDetailsComponent],
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
