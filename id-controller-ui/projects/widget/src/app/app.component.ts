import { Component, ElementRef, Input, ViewEncapsulation } from '@angular/core';
import { environment } from '../environments/environment';
import { WidgetModes } from '../../../id-controller-ui/src/app/enums/widget/widget-modes.enum';

@Component({
  selector: 'wdgt-identification',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class AppComponent {
  @Input()
  get mode(): WidgetModes {
    return this.widgetMode;
  }

  set mode(val) {
    if (val === WidgetModes.inline || val === WidgetModes.modal) {
      this.widgetMode = val;
    } else {
      this.widgetMode = WidgetModes.inline;
      console.error('Error. usrf-identification mode can only be "inline" or "modal". Using "inline" by default.');
    }
  }
  private widgetMode = WidgetModes.inline;

  constructor(private elementRef: ElementRef) {
    if (!environment.production) {
      // in webcomponent mode we can read prop assigned to app component.
      this.mode = this.elementRef.nativeElement.getAttribute('mode');
    }
  }
}
