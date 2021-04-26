import { Component, Inject } from '@angular/core';
import { WINDOW } from '@utils/injection-tokens';

@Component({
  selector: 'usrf-unauthenticated',
  templateUrl: './unauthenticated.component.html',
  styleUrls: ['./unauthenticated.component.scss'],
})
export class UnauthenticatedComponent {
  get showNavigation() {
    return !this.window.opener;
  }

  constructor(@Inject(WINDOW) private window: Window) {}
}
