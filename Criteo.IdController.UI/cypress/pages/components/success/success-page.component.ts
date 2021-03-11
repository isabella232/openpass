import { BaseComponent } from '../base.component';

export class SuccessPageComponent extends BaseComponent {
  constructor() {
    super('success');
  }

  getImage(name: string) {
    return this.getElement('img-' + name);
  }
}
