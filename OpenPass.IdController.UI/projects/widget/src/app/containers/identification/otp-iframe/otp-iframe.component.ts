import { AfterViewInit, Component, HostBinding, Inject, NgModule, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { environment } from '@env';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Observable, Subscription } from 'rxjs';
import { PostMessagesService } from '@services/post-messages.service';
import { filter, map, startWith } from 'rxjs/operators';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { MessageSubscriptionService } from '@services/message-subscription.service';
import { CookiesService } from '@services/cookies.service';
import { PublicApiService } from '@services/public-api.service';
import { CommonModule } from '@angular/common';
import { WINDOW } from '@utils/injection-tokens';
import { WidgetConfigurationService } from '@services/widget-configuration.service';

@Component({
  selector: 'wdgt-otp-iframe',
  templateUrl: './otp-iframe.component.html',
  styleUrls: ['./otp-iframe.component.scss'],
})
export class OtpIframeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('iframe') iframeElement: HTMLIFrameElement;

  @HostBinding('class.modal')
  get isModal(): boolean {
    return this.widgetConfigurationService.isModal && this.isOpen;
  }

  isOpen = true;
  iframeSrc$: Observable<SafeResourceUrl>;

  dynamicHeight$ = new Observable<number | undefined>();
  postSubscription: Subscription;

  constructor(
    @Inject(WINDOW) private window: Window,
    private sanitizer: DomSanitizer,
    private cookiesService: CookiesService,
    private publicApiService: PublicApiService,
    private postMessagesService: PostMessagesService,
    private messageSubscriptionService: MessageSubscriptionService,
    private widgetConfigurationService: WidgetConfigurationService
  ) {}

  ngOnInit() {
    this.iframeSrc$ = this.getIframeUrl();
    const { isDeclined } = this.publicApiService.getUserData();
    const hasCookie =
      this.cookiesService.getCookie(environment.cookieUid2Token) ||
      this.cookiesService.getCookie(environment.cookieIfaToken);
    this.isOpen = !hasCookie && !isDeclined;
    if (this.isOpen) {
      this.subscribeToOpenPass();
      this.listenForClosingRequest();
    }
  }

  ngOnDestroy() {
    this.messageSubscriptionService.destroyTokenListener();
    this.postSubscription?.unsubscribe?.();
  }

  ngAfterViewInit() {
    if (!this.isOpen) {
      return;
    }
    const iframeWindow = this.iframeElement.contentWindow as Window;
    this.messageSubscriptionService.initTokenListener(iframeWindow);
  }

  backdropClick() {
    if (this.widgetConfigurationService.isRequired) {
      return;
    }
    this.isOpen = false;
    this.publicApiService.setUserData({ ifaToken: null, uid2Token: null, isDeclined: true });
  }

  private subscribeToOpenPass() {
    this.dynamicHeight$ = this.postMessagesService.getSubscription().pipe(
      startWith({ height: 300, action: PostMessageActions.updateHeight }),
      filter((message) => message.action === PostMessageActions.updateHeight),
      map((message) => message.height)
    );
  }

  private listenForClosingRequest() {
    this.postSubscription = this.postMessagesService
      .getSubscription()
      .pipe(filter(({ action }) => action === PostMessageActions.closeChild))
      .subscribe(({ isDeclined }) => {
        this.publicApiService.setUserData({ isDeclined });
        this.isOpen = false;
      });
  }

  private getIframeUrl(): Observable<SafeResourceUrl> {
    return this.widgetConfigurationService.getConfiguration().pipe(
      map((configs) => {
        const query = new URLSearchParams({ origin: this.window.location.origin, ...configs });
        const iframeUrl = `${environment.idControllerAppUrl}?${query.toString()}`;
        return this.sanitizer.bypassSecurityTrustResourceUrl(iframeUrl);
      })
    );
  }
}

@NgModule({
  declarations: [OtpIframeComponent],
  imports: [CommonModule],
  exports: [OtpIframeComponent],
})
class OtpIframeModule {}
