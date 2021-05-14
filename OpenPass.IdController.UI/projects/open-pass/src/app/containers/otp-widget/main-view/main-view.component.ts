import { Component, OnDestroy, OnInit } from '@angular/core';
import { Actions, ofActionDispatched, Select } from '@ngxs/store';
import { Observable, Subscription } from 'rxjs';
import { OpenerState } from '@store/otp-widget/opener.state';
import { EventsTrackingService } from '@services/events-tracking.service';
import { EventTypes } from '@enums/event-types.enum';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { GetTokenByEmail, ReceiveToken } from '@store/otp-widget/auth.actions';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';

@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent implements OnInit, OnDestroy {
  @Select(OpenerState.originFormatted)
  websiteName$: Observable<string>;

  eventTypes = EventTypes;

  private authSubscriptions: Subscription;

  constructor(
    private actions$: Actions,
    private authService: AuthService,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  @Dispatch()
  getToken(email: string, eventType: EventTypes) {
    return new GetTokenByEmail(email, eventType);
  }

  ngOnInit() {
    this.authSubscriptions = this.actions$
      .pipe(ofActionDispatched(ReceiveToken))
      .subscribe(() => this.saveTokenAndClose());
    this.eventsTrackingService.trackEvent(EventTypes.bannerRequest);
  }

  ngOnDestroy() {
    this.authSubscriptions?.unsubscribe?.();
  }

  closeWindow() {
    this.dialogWindowService.closeDialogWindow(true);
  }

  private saveTokenAndClose() {
    this.authService.setTokenToOpener();
    this.eventsTrackingService.trackEvent(EventTypes.consentGranted);
    this.dialogWindowService.closeDialogWindow();
  }
}
