import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgreementViewComponent } from './agreement-view.component';
import { RouterTestingModule } from '@angular/router/testing';
import { PostMessagesService } from '@services/post-messages.service';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '@services/auth.service';
import { NgxsModule } from '@ngxs/store';
import { SharedModule } from '@components/shared/shared.module';

describe('AgreementViewComponent', () => {
  let component: AgreementViewComponent;
  let fixture: ComponentFixture<AgreementViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, TranslateModule.forRoot(), NgxsModule.forRoot(), SharedModule],
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
      declarations: [AgreementViewComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgreementViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
