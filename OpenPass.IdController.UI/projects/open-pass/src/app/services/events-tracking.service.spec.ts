import { TestBed } from '@angular/core/testing';

import { EventsTrackingService } from './events-tracking.service';
import { EventsService } from '@rest/events/events.service';
import { NgxsModule } from '@ngxs/store';
import { EventTypes } from '@shared/enums/event-types.enum';

describe('EventsTrackingService', () => {
  let service: EventsTrackingService;
  const restEventServiceMock = {
    trackEvent: (event: { eventType: EventTypes }) => ({ subscribe: () => {} }),
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: EventsService, useValue: restEventServiceMock }],
      imports: [NgxsModule.forRoot()],
    });
    service = TestBed.inject(EventsTrackingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call restEventService method', () => {
    const trackEventSpy = spyOn(restEventServiceMock, 'trackEvent').and.returnValue({ subscribe: () => {} });
    service.trackEvent(EventTypes.bannerRequest);
    expect(trackEventSpy).toHaveBeenCalledWith({ eventType: EventTypes.bannerRequest });
  });
});
