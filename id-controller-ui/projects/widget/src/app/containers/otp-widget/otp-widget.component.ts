import { Component, HostBinding, Inject, Input, OnInit } from '@angular/core';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { environment } from '../../../environments/environment';
import { EnvironmentService } from '../../services/environment.service';

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
  webComponentHost: string;

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

  constructor(@Inject('Window') private window: Window, private environmentService: EnvironmentService) {
    this.webComponentHost = environmentService.isPreprod
      ? 'https://my-advertising-experience.preprod.crto.in/open-pass/widget'
      : environment.webComponentHost;
  }

  ngOnInit(): void {}

  launchIdController() {
    this.window.open(environment.idControllerAppUrl, '_blank', this.openerConfigs);
  }
}
