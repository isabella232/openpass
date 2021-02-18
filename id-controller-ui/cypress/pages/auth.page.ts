import { BasePage } from './base-page';
import { AuthPageComponent } from './components/auth/auth-page.component';

export class AuthPage extends BasePage {
  pageUrl = '/auth';

  pageComponent = new AuthPageComponent();
}
