import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnloggedComponent } from './unlogged.component';
import { WINDOW } from '../../utils/injection-tokens';

describe('UnloggedComponent', () => {
  let component: UnloggedComponent;
  let fixture: ComponentFixture<UnloggedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [{ provide: WINDOW, useFactory: () => {} }],
      declarations: [UnloggedComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UnloggedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
