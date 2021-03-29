import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SsoViewComponent } from './sso-view.component';
import { WINDOW } from '@utils/injection-tokens';
import { GapiService } from '@services/gapi.service';
import { AuthenticatedService } from '@rest/authenticated/authenticated.service';
import { NgxsModule } from '@ngxs/store';
import { EventsTrackingService } from '@services/events-tracking.service';

const getMock = (serviceClass: any, mock = {}) => ({
  provide: serviceClass,
  useFactory: () => mock,
});

describe('SsoViewComponent', () => {
  let component: SsoViewComponent;
  let fixture: ComponentFixture<SsoViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot()],
      providers: [
        getMock(WINDOW, { gapi: {} }),
        getMock(AuthenticatedService),
        getMock(EventsTrackingService),
        getMock(GapiService, { load: () => new Promise(() => {}), renderButton: () => {} }),
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
