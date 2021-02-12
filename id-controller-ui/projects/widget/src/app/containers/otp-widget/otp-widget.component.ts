import { Component, HostBinding, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { environment } from '../../../environments/environment';
import { EnvironmentService } from '../../services/environment.service';
import { PostMessagesService } from '../../services/post-messages.service';
import { Subscription } from 'rxjs';
import { CookiesService } from '../../services/cookies.service';
import { PostMessageActions } from '../../../../../shared/enums/post-message-actions.enum';
import { PostMessagePayload } from '../../../../../shared/types/post-message-payload';

@Component({
  selector: 'wdgt-otp-widget',
  templateUrl: './otp-widget.component.html',
  styleUrls: ['./otp-widget.component.scss'],
})
export class OtpWidgetComponent implements OnInit, OnDestroy {
  @Input() mode: WidgetModes;

  @HostBinding('class.modal')
  get isModal(): boolean {
    return this.mode === WidgetModes.modal && this.isOpen;
  }

  isOpen = true;
  widgetMods = WidgetModes;
  websiteName = 'Website Name';
  webComponentHost: string;

  private messageSubscription: Subscription;

  get openerConfigs(): string {
    const { innerHeight, innerWidth } = this.window;
    const width = 400;
    const height = 500;
    const config = {
      width,
      height,
      left: (innerWidth - width) / 2,
      top: (innerHeight - height) / 2,
      location: environment.production ? 'no' : 'yes',
      toolbar: environment.production ? 'no' : 'yes',
    };
    return Object.entries(config)
      .map((entry) => entry.join('='))
      .join(',');
  }

  constructor(
    @Inject('Window') private window: Window,
    private cookiesService: CookiesService,
    private environmentService: EnvironmentService,
    private postMessageService: PostMessagesService
  ) {
    this.webComponentHost = environmentService.isPreprod
      ? 'https://my-advertising-experience.preprod.crto.in/open-pass/widget'
      : environment.webComponentHost;
  }

  ngOnInit() {
    const hasCookie = !!this.cookiesService.getCookie(environment.cookieName);
    this.isOpen = !hasCookie;
    if (hasCookie) {
      return;
    }
    this.waitForPostMessage();
  }

  ngOnDestroy() {
    this.postMessageService.stopListing();
    this.messageSubscription?.unsubscribe?.();
  }

  launchIdController() {
    const queryParams = new URLSearchParams({ origin: this.window.location.origin });
    const url = `${environment.idControllerAppUrl}?${queryParams}`;
    const openPassWindow = this.window.open(url, '_blank', this.openerConfigs);
    if (openPassWindow) {
      this.postMessageService.startListing(openPassWindow);
    }
  }

  private waitForPostMessage() {
    this.messageSubscription = this.postMessageService
      .getSubscription()
      .subscribe((payload) => this.setCookie(payload));
  }

  private setCookie(payload: PostMessagePayload) {
    if (payload.action === PostMessageActions.setToken) {
      this.isOpen = !payload.token;
      this.cookiesService.setCookie(environment.cookieName, payload.token, 31);
    }
  }
}
