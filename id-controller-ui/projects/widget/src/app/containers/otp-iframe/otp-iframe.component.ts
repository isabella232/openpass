import { AfterViewInit, Component, HostBinding, Inject, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { environment } from '../../../environments/environment';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Observable } from 'rxjs';
import { PostMessagesService } from '../../services/post-messages.service';
import { filter, map, startWith } from 'rxjs/operators';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { MessageSubscriptionService } from '../../services/message-subscription.service';

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

  constructor(
    @Inject('Window') window: Window,
    sanitizer: DomSanitizer,
    private postMessagesService: PostMessagesService,
    private messageSubscriptionService: MessageSubscriptionService
  ) {
    const iframeUrl = environment.idControllerAppUrl + '?origin=' + window.location.origin;
    this.iframeSrc = sanitizer.bypassSecurityTrustResourceUrl(iframeUrl);
  }

  ngOnInit() {
    this.subscribeToOpenPass();
  }

  ngOnDestroy() {
    this.messageSubscriptionService.destroyTokenListener();
  }

  ngAfterViewInit() {
    const iframeWindow = this.iframeElement.contentWindow as Window;
    this.messageSubscriptionService.initTokenListener(iframeWindow);
  }

  subscribeToOpenPass() {
    this.dynamicHeight$ = this.postMessagesService.getSubscription().pipe(
      startWith({ height: 300, action: PostMessageActions.updateHeight }),
      filter((message) => message.action === PostMessageActions.updateHeight),
      map((message) => message.height)
    );
  }
}
