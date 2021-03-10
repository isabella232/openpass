import { BaseComponent } from '../base.component';

export class AgreementPageComponent extends BaseComponent {
  constructor() {
    super('agreement');
  }

  getTitle() {
    return this.getElement('page-title');
  }

  getSummary() {
    return this.getElement('summary');
  }

  getUnifiedContent() {
    return this.getElement('unified-options');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }
}
