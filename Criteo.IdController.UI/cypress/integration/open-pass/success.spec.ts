import { SuccessPage } from '../../pages/success.page';

context('Success page', () => {
  let page;

  before(() => {
    page = new SuccessPage();
    page.goToPage();
  });

  it('should load images', () => {
    page.pageComponent
      .getImage('full-logo')
      .should('be.visible')
      .and((imgs) => expect(imgs[0]['naturalWidth']).to.be.greaterThan(0));
    page.pageComponent
      .getImage('small-logo')
      .should('be.visible') // cypress will wait for css animation
      .and((imgs) => expect(imgs[0]['naturalWidth']).to.be.greaterThan(0));
  });
});
