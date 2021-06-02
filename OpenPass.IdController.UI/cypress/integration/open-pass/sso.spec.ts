import { SsoPage } from '../../pages/sso.page';

context('SSO page', () => {
  let page: SsoPage;

  before(() => {
    page = new SsoPage();
    page.goToPage();
  });

  it('should render correct title', () => {
    page.pageComponent.getTitle().should('contain.text', 'Other options to access this site');
  });

  it('should render the google SSO button', () => {
    page.googleButtonComponent.getButton().should('contain.text', 'Login with Google');
  });

  it('should render the facebook button', () => {
    page.facebookButtonComponent.getButton().should('contain.text', 'Login with Facebook');
  });
});
