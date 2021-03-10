import { MainPage } from '../../pages/main.page';
import { CookiesHelper } from '../../helpers/cookies-helper';

context('Main page', () => {
  let page: MainPage;

  before(() => {
    page = new MainPage();
    page.goToPage();
  });

  it('should display a list of pros', () => {
    page.pageComponent.getProsItem().should('have.length', 3);
  });

  it('should display a button', () => {
    page.pageComponent.getActionBtn().should('be.visible');
  });

  it('should redirect to /auth', () => {
    page.pageComponent.getActionBtn().click();

    cy.location('pathname').should('be.eq', '/open-pass/auth');

    // reset state
    cy.go('back');
  });

  it('should redirect to /signed if cookie is present', () => {
    CookiesHelper.setAppToken();
    page.pageComponent.getActionBtn().click();

    cy.location('pathname').should('be.eq', '/open-pass/signed');

    // reset state
    cy.go('back');
    CookiesHelper.removeAppToken();
  });
});
