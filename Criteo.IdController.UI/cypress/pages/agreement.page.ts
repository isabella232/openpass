import { BasePage } from './base-page';
import { AgreementPageComponent } from './components/agreement/agreement-page.component';

export class AgreementPage extends BasePage {
  pageUrl = '/agreement';

  pageComponent = new AgreementPageComponent();
}
