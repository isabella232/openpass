import { EventTrackingHelper } from '../helpers/interceptors/event-tracking-helper';

export class BasePage {
  protected pageUrl = '/';

  constructor() {
    EventTrackingHelper.mockEventsTracking();
  }

  goToPage() {
    cy.visit(this.pageUrl);
  }
}
