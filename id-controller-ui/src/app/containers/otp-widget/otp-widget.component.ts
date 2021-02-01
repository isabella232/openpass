import { Component, Input, OnInit } from '@angular/core';
import { Views } from '@enums/widget/views.enum';
import { WidgetModes } from '@enums/widget/widget-modes.enum';

@Component({
  selector: 'usrf-otp-widget',
  templateUrl: './otp-widget.component.html',
  styleUrls: ['./otp-widget.component.scss'],
})
export class OtpWidgetComponent implements OnInit {
  @Input() mode: WidgetModes;

  widgetMods = WidgetModes;
  widgetViews = Views;
  currentView = Views.main;

  constructor() {}

  ngOnInit(): void {
    // TODO: initialize state
  }

  changeView(target: Views) {
    this.currentView = target;
  }
}
