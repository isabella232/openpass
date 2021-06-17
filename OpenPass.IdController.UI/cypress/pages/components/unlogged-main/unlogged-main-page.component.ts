import { BaseComponent } from '../base.component';

export class UnloggedMainPageComponent extends BaseComponent {
  constructor() {
    super('non-auth');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }

  getTermsCheckbox() {
    return this.getElement('terms-checkbox');
  }
}
