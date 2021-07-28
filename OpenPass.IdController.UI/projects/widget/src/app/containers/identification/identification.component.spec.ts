import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IdentificationComponent } from './identification.component';
import { ViewContainerModule } from '@directives/view-container.module';

describe('IdentificationComponent', () => {
  let component: IdentificationComponent;
  let fixture: ComponentFixture<IdentificationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [IdentificationComponent],
      imports: [ViewContainerModule],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IdentificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
