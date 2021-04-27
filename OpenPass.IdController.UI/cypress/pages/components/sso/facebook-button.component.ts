import { BaseComponent } from '../base.component';

export class FacebookButtonComponent extends BaseComponent {
  constructor() {
    super('f-button');
  }

  getButton() {
    return this.getElement('sign-in');
  }
}
