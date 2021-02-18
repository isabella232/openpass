import { BasePage } from './base-page';
import { SignedPageComponent } from './components/signed/signed-page.component';

export class SignedPage extends BasePage {
  pageUrl = '/signed';

  pageComponent = new SignedPageComponent();
}
