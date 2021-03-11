import { AuthPage } from '../../pages/auth.page';
import { LocalStorageHelper } from '../../helpers/local-storage-helper';

context('Auth Page', () => {
  let page = new AuthPage();

  context('with token', () => {
    before(() => {
      LocalStorageHelper.setFakeToken();
      page.goToPage();
    });

    it('should not have access to the page', () => {
      cy.location('pathname').should('not.equal', '/open-pass/auth');
    });
  });

  context('without token', () => {
    before(() => {
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
      cy.intercept('POST', '**/otp/generate', {}).as('generateCode');
      page.pageComponent.getActionBtn().click();
      cy.waitFor('@generateCode');

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
        cy.intercept('POST', '**/otp/generate', {}).as('generateCode');
        page.pageComponent.getActionBtn().click();

        cy.get('@generateCode').should('not.exist');
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
        cy.intercept('POST', '**/otp/generate', { statusCode: 422 }).as('generateCode');
        page.pageComponent.getEmailInput().type('invalid@email.com');
        page.pageComponent.getActionBtn().click();
        cy.waitFor('@generateCode');

        page.pageComponent.getEmailInput().should('have.class', 'invalid');
        page.pageComponent.getEmailWarning().should('be.visible');
      });
    });

    context('Code validation', () => {
      beforeEach(() => {
        cy.reload();
        cy.intercept('POST', '**/otp/generate', { statusCode: 200 }).as('generateCode');
        page.pageComponent.getEmailInput().type('valid@email.com');
        page.pageComponent.getActionBtn().click();
        cy.waitFor('@generateCode');
      });

      it('should allow only to type numbers', () => {
        page.pageComponent.getCodeInput().type('1lkjhhgfdfdsaxzcbmnytr2\\3%$^');
        page.pageComponent.getCodeInput().should('have.value', '123');
      });

      it("should display error if code isn't valid", () => {
        cy.intercept('POST', '**/otp/validate', { statusCode: 400 }).as('validateCode');
        page.pageComponent.getCodeInput().type('123456');
        page.pageComponent.getActionBtn().click();
        cy.waitFor('@validateCode');
        page.pageComponent.getCodeWarning().should('be.visible');
      });

      it('should redirect if code is valid', () => {
        cy.intercept('POST', '**/otp/validate', { statusCode: 200, body: { token: 'fake_token' } }).as('validateCode');
        page.pageComponent.getCodeInput().type('123456');
        page.pageComponent.getActionBtn().click();
        cy.waitFor('@validateCode');
        cy.location('pathname').should('be.eq', '/open-pass/agreement');
      });
    });
  });
});
