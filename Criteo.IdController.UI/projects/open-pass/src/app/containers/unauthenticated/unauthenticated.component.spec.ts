import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnauthenticatedComponent } from './unauthenticated.component';
import { WINDOW } from '@utils/injection-tokens';
import { windowFactory } from '../../../../../widget/src/app/utils/window-factory';

describe('UnauthenticatedComponent', () => {
  let component: UnauthenticatedComponent;
  let fixture: ComponentFixture<UnauthenticatedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UnauthenticatedComponent],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
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
});
