import { Component } from '@angular/core';
import { WidgetModes } from './enums/widget-modes.enum';
import { Variants } from './enums/variants.enum';
import { Sessions } from './enums/sessions.enum';
import { Providers } from './enums/providers.enum';

@Component({
  selector: 'wdgt-app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  viewTypes = WidgetModes;
  variantTypes = Variants;
  sessionTypes = Sessions;
  providerTypes = Providers;
}
