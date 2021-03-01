import {
  Component,
  ElementRef,
  Input,
  Output,
  ViewEncapsulation,
  EventEmitter,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { environment } from '../environments/environment';
import { WidgetModes } from './enums/widget-modes.enum';
import { Variants } from './enums/variants.enum';
import { UserData } from '@shared/types/public-api/user-data';
import { PublicApiService } from './services/public-api.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'wdgt-identification',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class AppComponent implements OnInit, OnDestroy {
  @Output()
  signUp = new EventEmitter<UserData>();
  @Output()
  loaded = new EventEmitter<void>();

  @Input() variant = Variants.dialog;

  @Input()
  get view(): WidgetModes {
    return this.widgetMode;
  }

  set view(val) {
    if (val === WidgetModes.inline || val === WidgetModes.modal) {
      this.widgetMode = val;
    } else {
      this.widgetMode = WidgetModes.inline;
      // eslint-disable-next-line no-console
      console.info('usrf-identification mode can only be "inline" or "modal". Using "inline" by default.');
    }
  }

  variantsList = Variants;
  userDataSubscription: Subscription;
  private widgetMode = WidgetModes.inline;

  constructor(private elementRef: ElementRef, private publicApiService: PublicApiService) {
    if (!environment.production) {
      // in webcomponent mode we can read prop assigned to app component.
      this.view = this.elementRef.nativeElement.getAttribute('view');
      this.variant = this.elementRef.nativeElement.getAttribute('variant');
    }
    this.elementRef.nativeElement.getUserData = this.getUserData.bind(this);
  }

  ngOnInit() {
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
}
