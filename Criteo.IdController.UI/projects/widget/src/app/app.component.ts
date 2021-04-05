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

  @Input()
  get view(): WidgetModes {
    return this.widgetMode;
  }

  set view(val) {
    if (val === WidgetModes.native || val === WidgetModes.modal) {
      this.widgetMode = val;
    } else {
      this.widgetMode = WidgetModes.native;
      // eslint-disable-next-line no-console
      console.info('usrf-identification mode can only be "native" or "modal". Using "native" by default.');
    }
  }

  userDataSubscription: Subscription;
  private widgetMode = WidgetModes.native;

  constructor(
    private elementRef: ElementRef,
    private publicApiService: PublicApiService,
    private injector: Injector,
    private componentFactoryResolver: ComponentFactoryResolver
  ) {
    if (!environment.production) {
      // in webcomponent mode we can read prop assigned to app component.
      this.view = this.elementRef.nativeElement.getAttribute('view') ?? this.view;
      this.variant = this.elementRef.nativeElement.getAttribute('variant') ?? this.variant;
      this.session = this.elementRef.nativeElement.getAttribute('session') ?? this.session;
    }
    this.elementRef.nativeElement.getUserData = this.getUserData.bind(this);
  }

  ngOnInit() {
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
    const componentRef = this.viewElement.viewContainerRef.createComponent<any>(componentFactory);

    componentRef.instance.view = this.view;
  }

  private async getComponentClass() {
    /* eslint-disable @typescript-eslint/naming-convention */
    switch (true) {
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
}
