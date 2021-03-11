import { BasePage } from './base-page';
import { UnloggedMainPageComponent } from './components/unlogged-main/unlogged-main-page.component';

export class UnloggedMainPage extends BasePage {
  pageUrl = '/unauthenticated';

  pageComponent = new UnloggedMainPageComponent();
}
