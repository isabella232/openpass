import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthViewComponent } from './auth-view.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NgxsModule } from '@ngxs/store';
import { AuthState } from '@store/otp-widget/auth.state';

describe('AuthViewComponent', () => {
  let component: AuthViewComponent;
  let fixture: ComponentFixture<AuthViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule, NgxsModule.forRoot([AuthState])],
      declarations: [AuthViewComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
