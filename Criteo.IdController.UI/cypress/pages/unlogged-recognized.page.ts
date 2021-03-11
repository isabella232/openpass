import { BasePage } from './base-page';
import { UnloggedRecognizedPageComponent } from './components/unlogged-recognized/unlogged-recognized-page.component';

export class UnloggedRecognizedPage extends BasePage {
  pageUrl = '/unauthenticated/recognized';

  pageComponent = new UnloggedRecognizedPageComponent();
}
