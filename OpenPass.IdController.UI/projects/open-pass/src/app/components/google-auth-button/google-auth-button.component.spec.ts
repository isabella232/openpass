import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GoogleAuthButtonComponent } from './google-auth-button.component';
import { NgxsModule } from '@ngxs/store';
import { stub } from '@utils/stub-factory';
import { GapiService } from '@services/gapi.service';

describe('GoogleAuthButtonComponent', () => {
  let component: GoogleAuthButtonComponent;
  let fixture: ComponentFixture<GoogleAuthButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxsModule.forRoot()],
      providers: [stub(GapiService, { load: () => new Promise(() => {}), renderButton: () => {} })],
      declarations: [GoogleAuthButtonComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GoogleAuthButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
