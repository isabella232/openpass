import { TestBed } from '@angular/core/testing';

import { EventsTrackingService } from './events-tracking.service';
import { EventsService } from '@rest/events/events.service';
import { NgxsModule } from '@ngxs/store';

describe('EventsTrackingService', () => {
  let service: EventsTrackingService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: EventsService,
          useFactory: () => {},
        },
      ],
      imports: [NgxsModule.forRoot()],
    });
    service = TestBed.inject(EventsTrackingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
