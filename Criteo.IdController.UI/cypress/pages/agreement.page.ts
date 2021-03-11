import { BasePage } from './base-page';
import { AgreementPageComponent } from './components/agreement/agreement-page.component';
import { OpenpassDetailsComponent } from './components/shared/openpass-details.component';

export class AgreementPage extends BasePage {
  pageUrl = '/agreement';

  pageComponent = new AgreementPageComponent();
  detailsComponent = new OpenpassDetailsComponent();
}
