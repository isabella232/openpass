import { Component, OnInit } from '@angular/core';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';
import { OpenerState } from '@store/otp-widget/opener.state';
import { EventsTrackingService } from '@services/events-tracking.service';
import { EventTypes } from '@enums/event-types.enum';

@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent implements OnInit {
  @Select(OpenerState.originFormatted)
  websiteName$: Observable<string>;

  constructor(private eventsTrackingService: EventsTrackingService) {}

  ngOnInit() {
    this.eventsTrackingService.trackEvent(EventTypes.bannerRequest);
  }
}
