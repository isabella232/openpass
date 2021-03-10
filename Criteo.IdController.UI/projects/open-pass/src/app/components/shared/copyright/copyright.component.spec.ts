import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CopyrightComponent } from './copyright.component';
import { TranslateModule } from '@ngx-translate/core';

describe('CopyrightComponent', () => {
  let component: CopyrightComponent;
  let fixture: ComponentFixture<CopyrightComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TranslateModule.forRoot()],
      declarations: [CopyrightComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CopyrightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
