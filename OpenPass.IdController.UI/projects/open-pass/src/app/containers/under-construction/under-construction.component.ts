import { Component, Inject } from '@angular/core';
import { WINDOW } from '../../utils/injection-tokens';

@Component({
  templateUrl: './under-construction.component.html',
  styleUrls: ['./under-construction.component.scss'],
})
export class UnderConstructionComponent {
  constructor(@Inject(WINDOW) private window: Window) {}

  back() {
    this.window.history.back();
  }
}
