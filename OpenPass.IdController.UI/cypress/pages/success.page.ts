import { BasePage } from './base-page';
import { SuccessPageComponent } from './components/success/success-page.component';

export class SuccessPage extends BasePage {
  pageUrl = '/success';

  pageComponent = new SuccessPageComponent();
}
