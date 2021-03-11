import { BaseComponent } from '../base.component';

export class UnloggedAgreementPageComponent extends BaseComponent {
  constructor() {
    super('unlogged-agreement');
  }

  getTitle() {
    return this.getElement('page-title');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }
}
