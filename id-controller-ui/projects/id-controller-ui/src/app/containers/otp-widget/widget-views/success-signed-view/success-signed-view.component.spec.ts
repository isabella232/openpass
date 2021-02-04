import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SuccessSignedViewComponent } from './success-signed-view.component';

describe('SuccessSignedViewComponent', () => {
  let component: SuccessSignedViewComponent;
  let fixture: ComponentFixture<SuccessSignedViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SuccessSignedViewComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SuccessSignedViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
