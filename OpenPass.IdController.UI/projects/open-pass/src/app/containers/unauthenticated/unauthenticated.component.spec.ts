import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnauthenticatedComponent } from './unauthenticated.component';
import { WINDOW } from '@utils/injection-tokens';
import { By } from '@angular/platform-browser';
import { Component } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';

@Component({
  selector: 'usrf-navigation',
  template: '',
})
class StubNavComponent {}

describe('UnauthenticatedComponent', () => {
  let component: UnauthenticatedComponent;
  let fixture: ComponentFixture<UnauthenticatedComponent>;
  const windowMock: Partial<Window> = { opener: true };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      declarations: [UnauthenticatedComponent, StubNavComponent],
      providers: [{ provide: WINDOW, useValue: windowMock }],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UnauthenticatedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show Navigation only in dialog mode', () => {
    let navPanel = fixture.debugElement.query(By.css('usrf-navigation'));
    expect(navPanel).toBeFalsy();
    windowMock.opener = null;
    fixture.detectChanges();
    navPanel = fixture.debugElement.query(By.css('usrf-navigation'));
    expect(navPanel).toBeTruthy();
  });
});
