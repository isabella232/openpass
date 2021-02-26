import { Component, ElementRef, Input, ViewEncapsulation } from '@angular/core';
import { environment } from '../environments/environment';
import { WidgetModes } from './enums/widget-modes.enum';
import { Variants } from './enums/variants.enum';

@Component({
  selector: 'wdgt-identification',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class AppComponent {
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
  private widgetMode = WidgetModes.inline;

  constructor(private elementRef: ElementRef) {
    if (!environment.production) {
      // in webcomponent mode we can read prop assigned to app component.
      this.view = this.elementRef.nativeElement.getAttribute('view');
      this.variant = this.elementRef.nativeElement.getAttribute('variant');
    }
  }
}
