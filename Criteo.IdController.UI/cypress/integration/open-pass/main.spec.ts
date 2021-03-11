import { MainPage } from '../../pages/main.page';
import { LocalStorageHelper } from '../../helpers/local-storage-helper';

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

  it('should redirect to /signed if token is present', () => {
    LocalStorageHelper.setFakeToken();
    page.pageComponent.getActionBtn().click();

    cy.location('pathname').should('be.eq', '/open-pass/signed');

    // reset state
    cy.go('back');
    LocalStorageHelper.clearLocalStorageItem('USRF');
  });
});
