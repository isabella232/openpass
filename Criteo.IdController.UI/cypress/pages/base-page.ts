export class BasePage {
  protected pageUrl = '/';

  goToPage() {
    cy.visit(this.pageUrl);
  }
}
