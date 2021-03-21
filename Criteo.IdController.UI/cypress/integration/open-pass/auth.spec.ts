import { AuthPage } from '../../pages/auth.page';
import { LocalStorageHelper } from '../../helpers/local-storage-helper';
import { AuthHelper } from '../../helpers/interceptors/auth-helper';

context('Auth Page', () => {
  let page: AuthPage;

  context('with token', () => {
    before(() => {
      page = new AuthPage();
      LocalStorageHelper.setFakeToken();
      page.goToPage();
    });

    it('should not have access to the page', () => {
      cy.location('pathname').should('not.equal', '/open-pass/auth');
    });
  });

  context('without token', () => {
    before(() => {
      page = new AuthPage();
      LocalStorageHelper.clearLocalStorageItem('USRF');
      page.goToPage();
    });

    it('should have access', () => {
      cy.location('pathname').should('be.equal', '/open-pass/auth');
    });

    it('should display content for 1 step', () => {
      page.pageComponent.getPageTitle().should('contain.text', 'Hi there!');
      page.pageComponent.getEmailInput().should('exist');
      page.pageComponent.getCodeInput().should('not.exist');
    });

    it('should display content for 2 step', () => {
      page.pageComponent.getEmailInput().type('valid@email.com');
      const waitingToken = AuthHelper.mockGenerateCode();
      page.pageComponent.getActionBtn().click();
      cy.waitFor(waitingToken);

      page.pageComponent.getPageTitle().should('contain.text', 'Thanks!');
      page.pageComponent.getEmailInput().should('exist');
      page.pageComponent.getCodeInput().should('exist');
      cy.reload();
    });

    context('Email validation', () => {
      beforeEach(() => {
        page.pageComponent.getEmailInput().clear();
      });

      it('should do not send request if there is no email', () => {
        const waitingToken = AuthHelper.mockGenerateCode();
        page.pageComponent.getActionBtn().click();

        cy.get(waitingToken).should('not.exist');
      });

      it('should show error if email is invalid', () => {
        page.pageComponent.getEmailInput().type('invalid email');
        page.pageComponent.getEmailWarning().should('be.visible');
        page.pageComponent.getEmailInput().clear().type('invalidemail@');
        page.pageComponent.getEmailWarning().should('be.visible');
        page.pageComponent.getEmailInput().clear().type('invalidemail@s.');
        page.pageComponent.getEmailWarning().should('be.visible');

        page.pageComponent.getEmailInput().clear();
        page.pageComponent.getEmailWarning().should('not.be.visible');
      });

      it('should show error if backend respond with error', () => {
        const waitingToken = AuthHelper.mockGenerateCode({ statusCode: 422 });
        page.pageComponent.getEmailInput().type('invalid@email.com');
        page.pageComponent.getActionBtn().click();
        cy.waitFor(waitingToken);

        page.pageComponent.getEmailInput().should('have.class', 'invalid');
        page.pageComponent.getEmailWarning().should('be.visible');
      });
    });

    context('Code validation', () => {
      beforeEach(() => {
        cy.reload();
        const waitingToken = AuthHelper.mockGenerateCode({ statusCode: 200 });
        page.pageComponent.getEmailInput().type('valid@email.com');
        page.pageComponent.getActionBtn().click();
        cy.waitFor(waitingToken);
      });

      it('should allow only to type numbers', () => {
        page.pageComponent.getCodeInput().type('1lkjhhgfdfdsaxzcbmnytr2\\3%$^');
        page.pageComponent.getCodeInput().should('have.value', '123');
      });

      it("should display error if code isn't valid", () => {
        const waitingToken = AuthHelper.mockValidateCode({ statusCode: 400 });
        page.pageComponent.getCodeInput().type('123456');
        page.pageComponent.getActionBtn().click();
        cy.waitFor(waitingToken);
        page.pageComponent.getCodeWarning().should('be.visible');
      });
    });
  });
});
