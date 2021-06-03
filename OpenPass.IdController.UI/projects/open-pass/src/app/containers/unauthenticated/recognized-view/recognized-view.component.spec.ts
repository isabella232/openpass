import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecognizedViewComponent } from './recognized-view.component';
import { PostMessagesService } from '@services/post-messages.service';
import { AuthService } from '@services/auth.service';
import { Component } from '@angular/core';

@Component({
  selector: 'usrf-copyright',
  template: '',
})
class StubCopyrightComponent {}

describe('RecognizedViewComponent', () => {
  let component: RecognizedViewComponent;
  let fixture: ComponentFixture<RecognizedViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [
        {
          provide: PostMessagesService,
          useFactory: () => {},
        },
        {
          provide: AuthService,
          useFactory: () => {},
        },
      ],
      declarations: [RecognizedViewComponent, StubCopyrightComponent],
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
