import { Injectable } from '@angular/core';
import { OpenerState } from '@store/otp-widget/opener.state';
import { Store } from '@ngxs/store';
import { EventTypes } from '@enums/event-types.enum';
import { EventsService } from '@rest/events/events.service';

@Injectable({
  providedIn: 'root',
})
export class EventsTrackingService {
  constructor(private eventsService: EventsService, private store: Store) {}

  trackEvent(eventType: EventTypes) {
    const originHost = this.store.selectSnapshot(OpenerState.origin);
    this.eventsService.trackEvent({ originHost, eventType }).subscribe();
  }
}
