import { UnloggedRecognizedPage } from '../../pages/unlogged-recognized.page';
import { LocalStorageHelper } from '../../helpers/local-storage-helper';

context('Unlogged:Recognized Page', () => {
  let page: UnloggedRecognizedPage;

  before(() => {
    page = new UnloggedRecognizedPage();
    LocalStorageHelper.setFakeToken();
    page.goToPage();
  });

  it('should display correct header', () => {
    page.pageComponent.getTitle().should('contain.text', 'Hi there!');
  });

  it('should send close request', () => {
    cy.spy(window.parent, 'postMessage');

    page.pageComponent
      .getActionBtn()
      .click()
      .should(() => expect(window.parent.postMessage).to.be.called);
  });
});
