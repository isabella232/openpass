import { Component, HostBinding, Inject, Input, OnInit } from '@angular/core';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'wdgt-otp-widget',
  templateUrl: './otp-widget.component.html',
  styleUrls: ['./otp-widget.component.scss'],
})
export class OtpWidgetComponent implements OnInit {
  @Input() mode: WidgetModes;

  @HostBinding('class.modal')
  get isModal(): boolean {
    return this.mode === WidgetModes.modal && this.isOpen;
  }

  isOpen = true;
  widgetMods = WidgetModes;
  websiteName = 'Website Name';

  get openerConfigs(): string {
    const { innerHeight, innerWidth } = this.window;
    const width = 400;
    const height = 500;
    const config = {
      width,
      height,
      left: (innerWidth - width) / 2,
      top: (innerHeight - height) / 2,
      location: environment.production ? 'no' : 'yes',
      toolbar: environment.production ? 'no' : 'yes',
    };
    return Object.entries(config)
      .map((entry) => entry.join('='))
      .join(',');
  }

  constructor(@Inject('Window') private window: Window) {}

  ngOnInit(): void {}

  launchIdController() {
    this.window.open('//localhost:4200/', 'blank', this.openerConfigs);
  }
}
