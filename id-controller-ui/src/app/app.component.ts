import { Component, Input, ViewEncapsulation } from '@angular/core';
import { WidgetModes } from '@enums/widget/widget-modes.enum';

@Component({
  selector: 'usrf-identification',
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
}
