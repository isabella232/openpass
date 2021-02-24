import { Component, HostBinding, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { environment } from '../../../environments/environment';
import { CookiesService } from '../../services/cookies.service';
import { MessageSubscriptionService } from '../../services/message-subscription.service';

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
  webComponentHost = environment.webComponentHost;

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
    private messageSubscriptionService: MessageSubscriptionService
  ) {}

  ngOnInit() {
    const hasCookie = !!this.cookiesService.getCookie(environment.cookieName);
    this.isOpen = !hasCookie;
  }

  ngOnDestroy() {
    this.messageSubscriptionService.destroyTokenListener();
  }

  launchIdController() {
    const queryParams = new URLSearchParams({ origin: this.window.location.origin });
    const url = `${environment.idControllerAppUrl}?${queryParams}`;
    const openPassWindow = this.window.open(url, '_blank', this.openerConfigs);
    if (openPassWindow) {
      this.messageSubscriptionService.initTokenListener(openPassWindow);
    }
  }
}
