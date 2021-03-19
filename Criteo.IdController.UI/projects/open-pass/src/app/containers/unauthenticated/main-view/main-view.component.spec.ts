import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainViewComponent } from './main-view.component';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxsModule } from '@ngxs/store';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '@services/auth.service';
import { PostMessagesService } from '@services/post-messages.service';
import { DialogWindowService } from '@services/dialog-window.service';

describe('MainViewComponent', () => {
  let component: MainViewComponent;
  let fixture: ComponentFixture<MainViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, NgxsModule.forRoot(), TranslateModule.forRoot()],
      providers: [
        {
          provide: AuthService,
          useFactory: () => {},
        },
        {
          provide: DialogWindowService,
          useFactory: () => {},
        },
      ],
      declarations: [MainViewComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MainViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
