import { SignedPage } from '../../pages/signed.page';
import { LocalStorageHelper } from '../../helpers/local-storage-helper';

context('Signed', () => {
  let page: SignedPage;
  const fakeEmail = 'JohnDoe@apple.com';

  before(() => {
    page = new SignedPage();
  });

  context('without token', () => {
    before(() => {
      LocalStorageHelper.clearLocalStorageItem('USRF');
      page.goToPage();
    });

    it('should be prohibited to open the page', () => {
      cy.location('pathname').should('be.eq', '/open-pass/auth');
    });
  });

  context('with token', () => {
    before(() => {
      LocalStorageHelper.setFakeToken();
      LocalStorageHelper.patchLocalStorage({ openpass: { email: fakeEmail } });
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

    it('should redirect to auth by clicking "Not me"', () => {
      page.pageComponent.getResetBtn().click();
      cy.location('pathname').should('be.eq', '/open-pass/auth');

      // reset state
      LocalStorageHelper.setFakeToken();
    });
  });
});
