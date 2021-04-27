import { UnloggedMainPage } from '../../pages/unlogged-main.page';
import { LocalStorageHelper } from '../../helpers/local-storage-helper';

context('Unlogged:Main page', () => {
  let page: UnloggedMainPage;

  before(() => {
    page = new UnloggedMainPage();
    page.goToPage();
  });

  it('should display a button', () => {
    page.pageComponent.getActionBtn().should('be.visible');
  });

  it('should redirect to /unauthenticated/recognized if token is present', () => {
    LocalStorageHelper.setFakeToken();
    page.goToPage();

    cy.location('pathname').should('be.eq', '/open-pass/unauthenticated/recognized');

    // reset state
    LocalStorageHelper.clearLocalStorageItem('USRF');
    page.goToPage();
  });
});
