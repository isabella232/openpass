import { AfterViewInit, Component, HostBinding, Inject, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { environment } from '../../../environments/environment';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Observable, Subscription } from 'rxjs';
import { PostMessagesService } from '../../services/post-messages.service';
import { filter, map, startWith } from 'rxjs/operators';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { MessageSubscriptionService } from '../../services/message-subscription.service';
import { CookiesService } from '../../services/cookies.service';

@Component({
  selector: 'wdgt-otp-iframe',
  templateUrl: './otp-iframe.component.html',
  styleUrls: ['./otp-iframe.component.scss'],
})
export class OtpIframeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('iframe') iframeElement: HTMLIFrameElement;
  @Input() view: WidgetModes;

  @HostBinding('class.modal')
  get isModal(): boolean {
    return this.view === WidgetModes.modal && this.isOpen;
  }

  isOpen = true;
  iframeSrc: SafeResourceUrl;

  dynamicHeight$ = new Observable<number | undefined>();
  postSubscription: Subscription;

  constructor(
    @Inject('Window') window: Window,
    sanitizer: DomSanitizer,
    private cookiesService: CookiesService,
    private postMessagesService: PostMessagesService,
    private messageSubscriptionService: MessageSubscriptionService
  ) {
    const iframeUrl = environment.idControllerAppUrl + '?origin=' + window.location.origin;
    this.iframeSrc = sanitizer.bypassSecurityTrustResourceUrl(iframeUrl);
  }

  ngOnInit() {
    this.isOpen = !this.cookiesService.getCookie(environment.cookieName);
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
      .subscribe(() => (this.isOpen = false));
  }
}
