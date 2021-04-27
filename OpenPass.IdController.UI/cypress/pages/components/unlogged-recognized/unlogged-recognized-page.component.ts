import { BaseComponent } from '../base.component';

export class UnloggedRecognizedPageComponent extends BaseComponent {
  constructor() {
    super('recognized');
  }

  getTitle() {
    return this.getElement('title');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }
}
