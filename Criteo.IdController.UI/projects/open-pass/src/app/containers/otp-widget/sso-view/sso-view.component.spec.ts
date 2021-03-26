import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SsoViewComponent } from './sso-view.component';
import { WINDOW } from '@utils/injection-tokens';
import { GapiService } from '@services/gapi.service';
import { AuthenticatedService } from '@rest/authenticated/authenticated.service';

describe('SsoViewComponent', () => {
  let component: SsoViewComponent;
  let fixture: ComponentFixture<SsoViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [
        { provide: WINDOW, useFactory: () => ({ gapi: {} }) },
        { provide: AuthenticatedService, useFactory: () => ({}) },
        {
          provide: GapiService,
          useFactory: () => ({ load: () => new Promise(() => {}), renderButton: () => {} }),
        },
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
