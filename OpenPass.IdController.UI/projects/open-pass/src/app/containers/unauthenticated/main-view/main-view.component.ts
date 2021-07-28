import { Component, OnInit } from '@angular/core';
import { Actions, ofActionDispatched, Select, Store } from '@ngxs/store';
import { OpenerState } from '@store/otp-widget/opener.state';
import { Observable } from 'rxjs';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { EventsService } from '@rest/events/events.service';
import { GetAnonymousTokens, GetAnonymousTokensSuccess } from '@store/otp-widget/auth.actions';
import { AuthState } from '@store/otp-widget/auth.state';
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
  @Select(AuthState.isFetching)
  isFetching$: Observable<boolean>;

  acceptTerms = false;

  constructor(
    private store: Store,
    private actions$: Actions,
    private authService: AuthService,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsService
  ) {}

  @Dispatch()
  fetchIfaAndProceed() {
    return new GetAnonymousTokens();
  }

  ngOnInit() {
    this.actions$
      .pipe(ofActionDispatched(GetAnonymousTokensSuccess), untilDestroyed(this))
      .subscribe(() => this.confirm());
    this.eventsTrackingService.trackEvent(EventTypes.bannerRequest).pipe(untilDestroyed(this)).subscribe();
  }

  closeWindow() {
    this.eventsTrackingService
      .trackEvent(EventTypes.consentNotGranted)
      .pipe(untilDestroyed(this))
      .subscribe(() => this.dialogWindowService.closeDialogWindow(true));
  }

  private confirm() {
    this.authService.setTokenToOpener();
    this.eventsTrackingService
      .trackEvent(EventTypes.consentGranted)
      .pipe(untilDestroyed(this))
      .subscribe(() => this.dialogWindowService.closeDialogWindow());
  }
}
