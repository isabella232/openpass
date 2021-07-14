import {
  Component,
  ComponentFactoryResolver,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { ViewContainerDirective } from '@directives/view-container.directive';
import { UserData } from '@shared/types/public-api/user-data';
import { Variants } from '@enums/variants.enum';
import { Sessions } from '@enums/sessions.enum';
import { Providers } from '@enums/providers.enum';
import { WidgetModes } from '@enums/widget-modes.enum';
import { Subscription } from 'rxjs';
import { CookiesService } from '@services/cookies.service';
import { PublicApiService } from '@services/public-api.service';
import { WidgetConfigurationService } from '@services/widget-configuration.service';
import { environment } from '@env';
import { WidgetConfiguration } from '@app-types/widget-configuration';

@Component({
  selector: 'wdgt-identification',
  templateUrl: './identification.component.html',
  styleUrls: ['./identification.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class IdentificationComponent implements OnInit, OnDestroy {
  @ViewChild(ViewContainerDirective, { static: true })
  viewElement: ViewContainerDirective;
  @Output()
  signUp = new EventEmitter<UserData>();
  @Output()
  loaded = new EventEmitter<void>();

  @Input() variant = Variants.dialog;
  @Input() session = Sessions.authenticated;
  @Input() provider = Providers.advertiser;

  @Input()
  get view(): WidgetModes {
    return this.widgetMode;
  }

  set view(val) {
    const definedValues = Object.values(WidgetModes);
    if (definedValues.includes(val)) {
      this.widgetMode = val;
    } else {
      this.widgetMode = WidgetModes.native;
      // eslint-disable-next-line no-console
      console.info(`usrf-identification view can only have one of these values: ${definedValues.join(', ')}`);
    }
  }

  userDataSubscription: Subscription;
  private widgetMode = WidgetModes.native;

  constructor(
    private elementRef: ElementRef,
    private cookiesService: CookiesService,
    private publicApiService: PublicApiService,
    private componentFactoryResolver: ComponentFactoryResolver,
    private widgetConfigurationService: WidgetConfigurationService
  ) {
    this.elementRef.nativeElement.getUserData = this.getUserData.bind(this);
  }

  ngOnInit() {
    this.saveConfiguration();
    this.loadComponent();
    this.userDataSubscription = this.publicApiService
      .getSubscription()
      .subscribe((userData) => this.signUp.emit(userData));
    this.loaded.emit();
  }

  ngOnDestroy() {
    this.userDataSubscription?.unsubscribe?.();
  }

  private getUserData(): UserData {
    return this.publicApiService.getUserData();
  }

  private async loadComponent() {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(
      (await this.getComponentClass()) as any
    );
    this.viewElement.viewContainerRef.createComponent(componentFactory);
  }

  private async getComponentClass() {
    /* eslint-disable @typescript-eslint/naming-convention */
    switch (true) {
      case this.variant === Variants.redirect:
        const { RedirectComponent } = await import('./redirect/redirect.component');
        return RedirectComponent;
      case this.session === Sessions.unauthenticated:
        const { UnloggedComponent } = await import('./unlogged/unlogged.component');
        return UnloggedComponent;
      case this.variant === Variants.inSite:
        const { OtpIframeComponent } = await import('./otp-iframe/otp-iframe.component');
        return OtpIframeComponent;

      default:
        const { OtpWidgetComponent } = await import('./otp-widget/otp-widget.component');
        return OtpWidgetComponent;
    }
    /* eslint-enable @typescript-eslint/naming-convention */
  }

  private saveConfiguration() {
    const config: WidgetConfiguration = {
      view: this.view,
      variant: this.variant,
      session: this.session,
      provider: this.provider,
      ifa: this.cookiesService.getCookie(environment.cookieIfaToken) ?? '',
      uid2: this.cookiesService.getCookie(environment.cookieUid2Token) ?? '',
      ctoBundle: this.cookiesService.getCookie('cto_bundle') ?? '',
    };
    this.widgetConfigurationService.setConfiguration(config);
  }
}
