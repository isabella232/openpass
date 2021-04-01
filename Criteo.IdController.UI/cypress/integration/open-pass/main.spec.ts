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

  it('should load images', () => {
    [
      page.pageComponent.getImage('logo'),
      page.pageComponent.getImage('info'),
      page.pageComponent.getImage('lock'),
      page.pageComponent.getImage('safe'),
    ].forEach((cyImage) => {
      cyImage.should('be.visible').and((imgs) => expect(imgs[0]['naturalWidth']).to.be.greaterThan(0));
    });
  });

  it('should redirect to /auth', () => {
    page.pageComponent.getActionBtn().click();

    cy.location('pathname').should('be.eq', '/open-pass/auth');

    // reset state
    cy.go('back');
  });

  it('should redirect to /signed if token is present', () => {
    LocalStorageHelper.setFakeToken();
    LocalStorageHelper.setFakeEmail();
    page.pageComponent.getActionBtn().click();

    cy.location('pathname').should('be.eq', '/open-pass/signed');

    // reset state
    cy.go('back');
    LocalStorageHelper.clearLocalStorageItem('USRF');
  });
});
