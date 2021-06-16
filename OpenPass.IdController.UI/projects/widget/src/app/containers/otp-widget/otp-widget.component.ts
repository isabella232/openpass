import { Component, HostBinding, Inject, NgModule, OnDestroy, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../environments/environment';
import { CookiesService } from '../../services/cookies.service';
import { MessageSubscriptionService } from '../../services/message-subscription.service';
import { filter, take } from 'rxjs/operators';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { Subscription } from 'rxjs';
import { PostMessagesService } from '../../services/post-messages.service';
import { PublicApiService } from '../../services/public-api.service';
import { CommonModule } from '@angular/common';
import { PipesModule } from '../../pipes/pipes.module';
import { WINDOW } from '../../utils/injection-tokens';
import { OpenPassDetailsModule } from '../../components/open-pass-details/open-pass-details.module';
import { EventTrackingService } from '../../rest/event-tracking/event-tracking.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { WidgetConfigurationService } from '../../services/widget-configuration.service';

@Component({
  selector: 'wdgt-otp-widget',
  templateUrl: './otp-widget.component.html',
  styleUrls: ['./otp-widget.component.scss'],
})
export class OtpWidgetComponent implements OnInit, OnDestroy {
  @HostBinding('class.modal')
  get isModal(): boolean {
    return this.widgetConfigurationService.isModal && this.isOpen;
  }

  get websiteName() {
    return this.window.location.host;
  }

  appHost = environment.appHost;
  isOpen = true;
  openPassWindow: Window;
  postSubscription: Subscription;

  get openerConfigs(): string {
    const { innerHeight, innerWidth, screenX, screenY } = this.window;
    const width = 400;
    const height = 520;
    const config = {
      width,
      height,
      left: (innerWidth - width) / 2 + screenX,
      top: (innerHeight - height) / 2 + screenY,
      location: environment.production ? 'no' : 'yes',
      toolbar: environment.production ? 'no' : 'yes',
    };
    return Object.entries(config)
      .map((entry) => entry.join('='))
      .join(',');
  }

  constructor(
    @Inject(WINDOW) private window: Window,
    private cookiesService: CookiesService,
    private publicApiService: PublicApiService,
    private postMessagesService: PostMessagesService,
    private eventTrackingService: EventTrackingService,
    private messageSubscriptionService: MessageSubscriptionService,
    public widgetConfigurationService: WidgetConfigurationService
  ) {}

  ngOnInit() {
    const hasCookie = !!this.cookiesService.getCookie(environment.cookieUid2Token);
    const { isDeclined } = this.publicApiService.getUserData();
    this.isOpen = !hasCookie && !isDeclined;
    if (this.isOpen) {
      this.eventTrackingService.track(EventTypes.bannerOpened).subscribe();
    }
  }

  ngOnDestroy() {
    this.messageSubscriptionService.destroyTokenListener();
    this.postSubscription?.unsubscribe?.();
  }

  launchIdController(path = '') {
    this.widgetConfigurationService
      .getConfiguration()
      .pipe(take(1))
      .subscribe((config) => {
        const queryParams = new URLSearchParams({ origin: this.window.location.origin, ...config });
        const url = `${environment.idControllerAppUrl}${path}?${queryParams}`;
        this.openPassWindow = this.window.open(url, '_blank', this.openerConfigs);
        if (this.openPassWindow) {
          this.messageSubscriptionService.initTokenListener(this.openPassWindow);
          this.listenForClosingRequest();
        }
      });
  }

  closeModal(force = false) {
    if (!force && this.widgetConfigurationService.isRequired) {
      return;
    }
    this.isOpen = false;
    this.publicApiService.setUserData({ ifaToken: null, uid2Token: null, isDeclined: true });
    this.eventTrackingService.track(EventTypes.bannerIgnored).subscribe();
  }

  private listenForClosingRequest() {
    this.postSubscription = this.postMessagesService
      .getSubscription()
      .pipe(filter(({ action }) => action === PostMessageActions.closeChild))
      .subscribe(() => {
        this.openPassWindow?.close();
        this.isOpen = false;
      });
  }
}

@NgModule({
  declarations: [OtpWidgetComponent],
  imports: [CommonModule, PipesModule, TranslateModule, OpenPassDetailsModule],
})
class OtpWidgetModule {}
