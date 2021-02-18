import { BaseComponent } from '../base.component';

export class SignedPageComponent extends BaseComponent {
  constructor() {
    super('signed');
  }

  getTitle() {
    return this.getElement('page-title');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }

  getResetBtn() {
    return this.getElement('reset-btn');
  }

  getEmailInput() {
    return this.getElement('user-email');
  }
}
