import { Component, HostBinding, Inject, Input, NgModule, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';
import { WINDOW } from '../../utils/injection-tokens';
import { CookiesService } from '../../services/cookies.service';
import { PublicApiService } from '../../services/public-api.service';
import { PostMessagesService } from '../../services/post-messages.service';
import { MessageSubscriptionService } from '../../services/message-subscription.service';
import { environment } from '../../../environments/environment';
import { filter } from 'rxjs/operators';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PipesModule } from '../../pipes/pipes.module';
import { OpenPassDetailsModule } from '../../components/open-pass-details/open-pass-details.module';

@Component({
  selector: 'wdgt-unlogged',
  templateUrl: './unlogged.component.html',
  styleUrls: ['./unlogged.component.scss'],
})
export class UnloggedComponent implements OnInit, OnDestroy {
  @Input() view: WidgetModes;

  get websiteName() {
    return this.window.location.host;
  }

  isOpen = true;
  widgetMods = WidgetModes;
  hasCookie = false;
  openPassWindow: Window;
  postSubscription: Subscription;

  @HostBinding('class.modal')
  get isModal(): boolean {
    return this.view === WidgetModes.modal;
  }

  @HostBinding('attr.hidden')
  get isHidden() {
    return !this.isOpen || this.hasCookie;
  }

  get openerConfigs(): string {
    const { innerHeight, innerWidth, screenX, screenY } = this.window;
    const width = 450;
    const height = 745;
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
    private messageSubscriptionService: MessageSubscriptionService
  ) {}

  ngOnInit() {
    this.hasCookie = !!this.cookiesService.getCookie(environment.cookieUserToken);
  }

  ngOnDestroy() {
    this.messageSubscriptionService.destroyTokenListener();
    this.postSubscription?.unsubscribe?.();
  }

  backdropClick() {
    this.isOpen = false;
    this.publicApiService.setUserData({ token: null, isDeclined: true });
  }

  launchOpenPassApp() {
    const appPath = new URL(environment.idControllerAppUrl);
    appPath.pathname += environment.unloggedPath;
    appPath.searchParams.set('origin', this.window.location.origin);
    this.openPassWindow = this.window.open(appPath.toString(), '_blank', this.openerConfigs);
    if (this.openPassWindow) {
      this.messageSubscriptionService.initTokenListener(this.openPassWindow);
      this.listenForClosingRequest();
    }
  }

  private listenForClosingRequest() {
    this.postSubscription = this.postMessagesService
      .getSubscription()
      .pipe(filter(({ action }) => action === PostMessageActions.closeChild))
      .subscribe(() => {
        this.isOpen = false;
        this.openPassWindow?.close();
      });
  }
}

@NgModule({
  declarations: [UnloggedComponent],
  imports: [CommonModule, PipesModule, TranslateModule, OpenPassDetailsModule],
})
class UnloggedModule {}
