import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecognizedViewComponent } from './recognized-view.component';

describe('RecognizedViewComponent', () => {
  let component: RecognizedViewComponent;
  let fixture: ComponentFixture<RecognizedViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RecognizedViewComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RecognizedViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
