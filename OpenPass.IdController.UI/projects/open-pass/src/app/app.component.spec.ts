import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { windowFactory } from '@utils/window-factory';
import { NgxsModule } from '@ngxs/store';
import { TranslateModule } from '@ngx-translate/core';
import { WINDOW } from '@utils/injection-tokens';
import { stub } from '@utils/stub-factory';
import { PostMessagesService } from '@services/post-messages.service';
import { NgxsDispatchPluginModule } from '@ngxs-labs/dispatch-decorator';

describe('AppComponent', () => {
  const postMessagesServiceStub = { startListening: () => {} };
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        HttpClientTestingModule,
        NgxsModule.forRoot([]),
        TranslateModule.forRoot(),
        NgxsDispatchPluginModule,
      ],
      providers: [stub(PostMessagesService, postMessagesServiceStub), { provide: WINDOW, useFactory: windowFactory }],
      declarations: [AppComponent],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should call PostMessagesService.startListening() on init', () => {
    const listener = spyOn(postMessagesServiceStub, 'startListening');
    const fixture = TestBed.createComponent(AppComponent);
    fixture.componentInstance.ngOnInit();
    expect(listener).toHaveBeenCalled();
  });
});
