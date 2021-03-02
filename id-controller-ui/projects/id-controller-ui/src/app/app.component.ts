import { Component, Inject, OnInit } from '@angular/core';
import { PostMessagesService } from '@services/post-messages.service';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { SaveOpener } from '@store/otp-widget/opener.actions';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'usrf-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(
    private postMessageService: PostMessagesService,
    @Inject('Window') private window: Window,
    private translateService: TranslateService
  ) {}

  @Dispatch()
  private recognizeOrigin() {
    const searchParams = new URLSearchParams(this.window.location.search);
    const origin = searchParams.get('origin');
    return new SaveOpener(origin);
  }

  ngOnInit() {
    this.recognizeOrigin();
    this.postMessageService.startListening();
    this.applyLanguage();
  }

  private applyLanguage() {
    const supportedLanguages = ['en'];
    const userLanguage = this.translateService.getBrowserLang();
    if (supportedLanguages.includes(userLanguage) && userLanguage !== this.translateService.getDefaultLang()) {
      this.translateService.use(userLanguage);
    }
  }
}
