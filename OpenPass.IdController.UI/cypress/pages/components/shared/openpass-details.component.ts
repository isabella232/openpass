import { BaseComponent } from '../base.component';

export class OpenpassDetailsComponent extends BaseComponent {
  constructor() {
    super('openpass-details');
  }

  getSummary() {
    return this.getElement('summary');
  }

  getUnifiedContent() {
    return this.getElement('unified-options');
  }
}
