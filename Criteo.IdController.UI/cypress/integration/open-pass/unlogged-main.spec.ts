import { UnloggedMainPage } from '../../pages/unlogged-main.page';
import { LocalStorageHelper } from '../../helpers/local-storage-helper';

context('Unlogged:Main page', () => {
  let page: UnloggedMainPage;

  before(() => {
    page = new UnloggedMainPage();
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

  it('should redirect to /unauthenticated/recognized if token is present', () => {
    LocalStorageHelper.setFakeToken();
    page.goToPage();

    cy.location('pathname').should('be.eq', '/open-pass/unauthenticated/recognized');

    // reset state
    LocalStorageHelper.clearLocalStorageItem('USRF');
    page.goToPage();
  });
});
