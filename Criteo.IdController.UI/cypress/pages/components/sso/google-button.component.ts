import { BaseComponent } from '../base.component';

export class GoogleButtonComponent extends BaseComponent {
  constructor() {
    super('g-button');
  }

  getButton() {
    return this.getElement('sign-in');
  }
}
