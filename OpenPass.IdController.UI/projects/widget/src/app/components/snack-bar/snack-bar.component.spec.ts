import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SnackBarComponent } from './snack-bar.component';
import { PipesModule } from '@pipes/pipes.module';
import { DEPLOY_URL } from '@utils/injection-tokens';

describe('SnackBarComponent', () => {
  let component: SnackBarComponent;
  let fixture: ComponentFixture<SnackBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SnackBarComponent],
      imports: [PipesModule],
      providers: [{ provide: DEPLOY_URL, useFactory: () => {} }],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SnackBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
