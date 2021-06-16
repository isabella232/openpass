import {
  Component,
  ComponentFactoryResolver,
  ElementRef,
  EventEmitter,
  Injector,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { environment } from '../environments/environment';
import { WidgetModes } from './enums/widget-modes.enum';
import { Variants } from './enums/variants.enum';
import { UserData } from '@shared/types/public-api/user-data';
import { PublicApiService } from './services/public-api.service';
import { Subscription } from 'rxjs';
import { Sessions } from './enums/sessions.enum';
import { ViewContainerDirective } from './directives/view-container.directive';
import { TranslateService } from '@ngx-translate/core';
import { Providers } from './enums/providers.enum';
import { WidgetConfiguration } from './types/widget-configuration';
import { WidgetConfigurationService } from './services/widget-configuration.service';
import { CookiesService } from './services/cookies.service';

@Component({
  selector: 'wdgt-identification',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class AppComponent implements OnInit, OnDestroy {
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
    private injector: Injector,
    private elementRef: ElementRef,
    private cookiesService: CookiesService,
    private publicApiService: PublicApiService,
    private translateService: TranslateService,
    private componentFactoryResolver: ComponentFactoryResolver,
    private widgetConfigurationService: WidgetConfigurationService
  ) {
    if (!environment.production) {
      // in webcomponent mode we can read prop assigned to app component.
      this.view = this.elementRef.nativeElement.getAttribute('view') ?? this.view;
      this.variant = this.elementRef.nativeElement.getAttribute('variant') ?? this.variant;
      this.session = this.elementRef.nativeElement.getAttribute('session') ?? this.session;
      this.provider = this.elementRef.nativeElement.getAttribute('provider') ?? this.provider;
    }
    this.elementRef.nativeElement.getUserData = this.getUserData.bind(this);
  }

  ngOnInit() {
    this.applyLanguage();
    this.saveConfiguration();
    this.loadComponent();
    const isDev = !environment.production;
    this.userDataSubscription = this.publicApiService.getSubscription().subscribe((userData) => {
      this.signUp.emit(userData);
      if (isDev) {
        const event = new CustomEvent('signUp', { detail: userData });
        this.elementRef.nativeElement.dispatchEvent(event);
      }
    });
    this.loaded.emit();
    if (isDev) {
      this.elementRef.nativeElement.dispatchEvent(new Event('loaded'));
    }
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
        const { RedirectComponent } = await import('./containers/redirect/redirect.component');
        return RedirectComponent;
      case this.session === Sessions.unauthenticated:
        const { UnloggedComponent } = await import('./containers/unlogged/unlogged.component');
        return UnloggedComponent;
      case this.variant === Variants.inSite:
        const { OtpIframeComponent } = await import('./containers/otp-iframe/otp-iframe.component');
        return OtpIframeComponent;

      default:
        const { OtpWidgetComponent } = await import('./containers/otp-widget/otp-widget.component');
        return OtpWidgetComponent;
    }
    /* eslint-enable @typescript-eslint/naming-convention */
  }

  private applyLanguage() {
    const supportedLanguages = ['en', 'ja'];
    const userLanguage = this.translateService.getBrowserLang();
    if (supportedLanguages.includes(userLanguage) && userLanguage !== this.translateService.getDefaultLang()) {
      this.translateService.use(userLanguage);
    }
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
