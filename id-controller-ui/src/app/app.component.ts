import { Component, Input } from '@angular/core';

@Component({
  selector: 'usrf-identification',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  @Input()
  get mode(): 'inline' | 'modal' {
    return this.widgetMode;
  }
  set mode(val) {
    if (val === 'inline' || val === 'modal') {
      this.widgetMode = val;
    } else {
      this.widgetMode = 'inline';
      console.error('Error. usrf-identification mode can only be "inline" or "modal". Using "inline" by default.');
    }
  }
  private widgetMode: 'inline' | 'modal' = 'inline';
}
