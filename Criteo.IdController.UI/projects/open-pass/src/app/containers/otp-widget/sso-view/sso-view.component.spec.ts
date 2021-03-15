import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SsoViewComponent } from './sso-view.component';
import { WINDOW } from '@utils/injection-tokens';
import { GapiService } from '@services/gapi.service';

describe('SsoViewComponent', () => {
  let component: SsoViewComponent;
  let fixture: ComponentFixture<SsoViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [
        { provide: WINDOW, useFactory: () => ({ gapi: {} }) },
        {
          provide: GapiService,
          useFactory: () => ({ load: () => {}, renderButton: () => {} }),
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
