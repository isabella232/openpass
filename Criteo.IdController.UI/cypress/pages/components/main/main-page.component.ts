import { BaseComponent } from '../base.component';

export class MainPageComponent extends BaseComponent {
  constructor() {
    super('main');
  }

  getActionBtn() {
    return this.getElement('action-btn');
  }

  getProsItem() {
    return this.getElement('pros-item');
  }
}
