import { Component, OnInit } from '@angular/core';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';
import { OpenerState } from '@store/otp-widget/opener.state';
import { EventsService } from '@rest/events/events.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { DialogWindowService } from '@services/dialog-window.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent implements OnInit {
  @Select(OpenerState.originFormatted)
  websiteName$: Observable<string>;

  constructor(private dialogWindowService: DialogWindowService, private eventsTrackingService: EventsService) {}

  ngOnInit() {
    this.eventsTrackingService.trackEvent(EventTypes.bannerRequest).pipe(untilDestroyed(this)).subscribe();
  }

  closeWindow() {
    this.dialogWindowService.closeDialogWindow(true);
  }
}
