export class BaseComponent {
  constructor(public prefix: string) {}

  getWrapper() {
    return this.getElement('wrapper');
  }

  getElement(attributeName: string) {
    return this.getElementByDataAttr(`${this.prefix}-${attributeName}`);
  }

  getElementByDataAttr(attributeName: string) {
    const attr = `[data-test="${attributeName}"]`;
    return cy.get(attr);
  }
}
