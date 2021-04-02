import { BaseComponent } from '../base.component';

export class SsoPageComponent extends BaseComponent {
  constructor() {
    super('sso');
  }

  getTitle() {
    return this.getElement('page-title');
  }
}
