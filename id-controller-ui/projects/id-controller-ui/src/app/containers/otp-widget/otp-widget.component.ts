import { Component, Inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { PostMessagesService } from '@services/post-messages.service';
import { DOCUMENT } from '@angular/common';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { delay, filter } from 'rxjs/operators';
import { WINDOW } from '@utils/injection-tokens';

@Component({
  selector: 'usrf-otp-widget',
  templateUrl: './otp-widget.component.html',
  styleUrls: ['./otp-widget.component.scss'],
})
export class OtpWidgetComponent {
  constructor(
    @Inject(DOCUMENT) private document: Document,
    @Inject(WINDOW) private window: Window,
    router: Router,
    postMessagesService: PostMessagesService
  ) {
    if (this.window.opener) {
      return;
    }
    router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        delay(25) // waiting for render
      )
      .subscribe(() => {
        postMessagesService.sendMessage({
          action: PostMessageActions.updateHeight,
          height: this.document.body.scrollHeight,
        });
      });
  }
}
