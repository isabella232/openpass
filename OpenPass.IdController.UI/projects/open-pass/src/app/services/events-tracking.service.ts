import { Injectable } from '@angular/core';
import { EventTypes } from '@shared/enums/event-types.enum';
import { EventsService } from '@rest/events/events.service';

@Injectable({
  providedIn: 'root',
})
export class EventsTrackingService {
  constructor(private eventsService: EventsService) {}

  trackEvent(eventType: EventTypes) {
    this.eventsService.trackEvent({ eventType }).subscribe();
  }
}
