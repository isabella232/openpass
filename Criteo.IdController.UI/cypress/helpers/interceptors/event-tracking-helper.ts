enum EventTracking {
  trackEvent = 'trackEvent',
}

export class EventTrackingHelper {
  static mockEventsTracking() {
    cy.intercept('POST', '**/event').as(EventTracking.trackEvent);
  }
}
