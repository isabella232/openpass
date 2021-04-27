import { BaseComponent } from '../base.component';

export class AgreementPageComponent extends BaseComponent {
  constructor() {
    super('agreement');
  }

  getTitle() {
    return this.getElement('page-title');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }
}
