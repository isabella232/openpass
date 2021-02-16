import { BasePage } from './proto/base-page';
import { MainPageComponent } from '../components/main/main-page.component';

export class MainPage extends BasePage {
  pageUrl = '/';

  pageComponent = new MainPageComponent();
}
