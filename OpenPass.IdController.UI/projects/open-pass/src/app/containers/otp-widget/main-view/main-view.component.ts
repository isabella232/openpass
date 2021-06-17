import { Component, OnInit } from '@angular/core';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';
import { OpenerState } from '@store/otp-widget/opener.state';
import { EventsTrackingService } from '@services/events-tracking.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { DialogWindowService } from '@services/dialog-window.service';

@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent implements OnInit {
  @Select(OpenerState.originFormatted)
  websiteName$: Observable<string>;

  constructor(private dialogWindowService: DialogWindowService, private eventsTrackingService: EventsTrackingService) {}

  ngOnInit() {
    this.eventsTrackingService.trackEvent(EventTypes.bannerRequest);
  }

  closeWindow() {
    this.dialogWindowService.closeDialogWindow(true);
  }
}
