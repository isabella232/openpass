import { SignedPage } from '../../pages/signed.page';
import { CookiesHelper } from '../../helpers/cookies-helper';

context('Signed', () => {
  let page: SignedPage;
  const fakeEmail = 'JohnDoe@apple.com';

  before(() => {
    page = new SignedPage();
  });

  context('without token', () => {
    before(() => {
      CookiesHelper.removeAppToken();
      page.goToPage();
    });

    it('should be prohibited to open the page', () => {
      cy.location('pathname').should('be.eq', '/open-pass/auth');
    });
  });

  context('with token', () => {
    before(() => {
      CookiesHelper.setAppToken();
      window.localStorage.setItem('USRF.openpass.email', fakeEmail);
      page.goToPage();
    });

    it('should be allowed to open the page', () => {
      cy.location('pathname').should('be.eq', '/open-pass/signed');
    });

    it('should has correct title', () => {
      page.pageComponent.getTitle().should('contain.text', 'Hi there!');
    });

    it('should contains correct user Email', () => {
      page.pageComponent.getEmailInput().should('have.value', fakeEmail);
    });
  });
});
