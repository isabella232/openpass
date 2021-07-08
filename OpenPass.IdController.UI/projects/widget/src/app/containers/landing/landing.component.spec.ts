import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LandingComponent } from './landing.component';
import { UseDeployUrlPipe } from '@pipes/use-deploy-url.pipe';
import { DEPLOY_URL } from '@utils/injection-tokens';
import { PipesModule } from '@pipes/pipes.module';

describe('LandingComponent', () => {
  let component: LandingComponent;
  let fixture: ComponentFixture<LandingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LandingComponent],
      imports: [PipesModule],
      providers: [
        { provide: UseDeployUrlPipe, useValue: { transform: () => {} } },
        { provide: DEPLOY_URL, useFactory: () => {} },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LandingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
