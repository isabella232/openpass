import { UnloggedAgreementPage } from '../../pages/unlogged-agreement.page';

context('Unlogged:Agreement Page', () => {
  let page: UnloggedAgreementPage;

  before(() => {
    page = new UnloggedAgreementPage();
    page.goToPage();
  });

  return true; // page is disabled
  it('should display correct title', () => {
    page.pageComponent.getTitle().should('contain.text', 'Awesome!');
  });

  it('should toggle details', () => {
    page.detailsComponent.getUnifiedContent().should('not.be.visible');
    page.detailsComponent.getSummary().click();
    page.detailsComponent.getUnifiedContent().should('be.visible');
    page.detailsComponent.getSummary().click();
    page.detailsComponent.getUnifiedContent().should('not.be.visible');
  });

  it('should send close request', () => {
    cy.spy(window.parent, 'postMessage');

    page.pageComponent
      .getActionBtn()
      .click()
      .should(() => expect(window.parent.postMessage).to.be.called);
  });
});
