import { BasePage } from './base-page';
import { UnloggedAgreementPageComponent } from './components/unlogged-agreement/unlogged-agreement-page.component';
import { OpenpassDetailsComponent } from './components/shared/openpass-details.component';

export class UnloggedAgreementPage extends BasePage {
  pageUrl = '/unauthenticated/agreement';

  pageComponent = new UnloggedAgreementPageComponent();
  detailsComponent = new OpenpassDetailsComponent();
}
