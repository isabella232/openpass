import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookAuthButtonComponent } from './facebook-auth-button.component';
import { stub } from '@utils/stub-factory';
import { WINDOW } from '@utils/injection-tokens';

describe('FacebookAuthButtonComponent', () => {
  let component: FacebookAuthButtonComponent;
  let fixture: ComponentFixture<FacebookAuthButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FacebookAuthButtonComponent],
      // eslint-disable-next-line @typescript-eslint/naming-convention
      providers: [stub(WINDOW, { FB: { init: () => {} } })],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookAuthButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
