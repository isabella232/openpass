import { BasePage } from './base-page';
import { SsoPageComponent } from './components/sso/sso-page.component';
import { GoogleButtonComponent } from './components/sso/google-button.component';
import { FacebookButtonComponent } from './components/sso/facebook-button.component';

export class SsoPage extends BasePage {
  pageUrl = '/sso';

  pageComponent = new SsoPageComponent();
  googleButtonComponent = new GoogleButtonComponent();
  facebookButtonComponent = new FacebookButtonComponent();
}
