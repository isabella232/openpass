import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnloggedComponent } from './unlogged.component';
import { DEPLOY_URL, WINDOW } from '../../utils/injection-tokens';
import { windowFactory } from '../../utils/window-factory';
import { PipesModule } from '../../pipes/pipes.module';

describe('UnloggedComponent', () => {
  let component: UnloggedComponent;
  let fixture: ComponentFixture<UnloggedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UnloggedComponent],
      imports: [PipesModule],
      providers: [
        { provide: WINDOW, useFactory: windowFactory },
        { provide: DEPLOY_URL, useFactory: () => {} },
      ],
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
