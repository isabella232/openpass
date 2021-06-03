import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtpWidgetComponent } from './otp-widget.component';
import { windowFactory } from '../../../../../widget/src/app/utils/window-factory';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxsModule } from '@ngxs/store';
import { WINDOW } from '@utils/injection-tokens';
import { Component } from '@angular/core';

@Component({
  selector: 'usrf-navigation',
  template: '',
})
class StubNavComponent {}

describe('OtpWidgetComponent', () => {
  let component: OtpWidgetComponent;
  let fixture: ComponentFixture<OtpWidgetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, NgxsModule.forRoot([])],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
      declarations: [OtpWidgetComponent, StubNavComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OtpWidgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
