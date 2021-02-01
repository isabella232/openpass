import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AlreadySignedViewComponent } from './already-signed-view.component';

describe('AlreadySignedViewComponent', () => {
  let component: AlreadySignedViewComponent;
  let fixture: ComponentFixture<AlreadySignedViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AlreadySignedViewComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AlreadySignedViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
