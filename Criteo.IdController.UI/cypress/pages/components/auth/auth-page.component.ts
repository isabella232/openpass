import { BaseComponent } from '../base.component';

export class AuthPageComponent extends BaseComponent {
  constructor() {
    super('auth');
  }

  getPageTitle() {
    return this.getElement('page-title');
  }

  getEmailInput() {
    return this.getElement('input-email');
  }

  getCodeInput() {
    return this.getElement('input-code');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }

  getEmailWarning() {
    return this.getElement('email-warning');
  }

  getCodeWarning() {
    return this.getElement('code-warning');
  }

  getAgreementCheckbox() {
    return this.getElement('agreement');
  }

  checkCheckbox(value?) {
    this.getAgreementCheckbox().check(value);
  }
}
