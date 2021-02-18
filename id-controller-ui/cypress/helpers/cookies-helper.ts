import SetCookieOptions = Cypress.SetCookieOptions;
import { environment } from '../../projects/id-controller-ui/src/environments/environment';

export class CookiesHelper {
  static setCookie(name: string, value: string, options?: Partial<SetCookieOptions>) {
    return cy.setCookie(name, value, options);
  }

  static setAppToken() {
    return CookiesHelper.setCookie(environment.cookieName, 'fake-value');
  }

  static removeAppToken() {
    return cy.clearCookie(environment.cookieName);
  }
}
