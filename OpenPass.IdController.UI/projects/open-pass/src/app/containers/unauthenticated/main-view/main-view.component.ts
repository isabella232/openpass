import { Component, OnDestroy, OnInit } from '@angular/core';
import { Actions, ofActionDispatched, Select, Store } from '@ngxs/store';
import { OpenerState } from '@store/otp-widget/opener.state';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { EventTypes } from '@enums/event-types.enum';
import { EventsTrackingService } from '@services/events-tracking.service';
import { GetAnonymousTokens, GetAnonymousTokensSuccess } from '@store/otp-widget/auth.actions';
import { AuthState } from '@store/otp-widget/auth.state';

@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent implements OnInit, OnDestroy {
  @Select(OpenerState.originFormatted)
  websiteName$: Observable<string>;
  @Select(AuthState.isFetching)
  isFetching$: Observable<boolean>;

  isDestroyed = new Subject();
  acceptTerms = false;

  constructor(
    private store: Store,
    private actions$: Actions,
    private authService: AuthService,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  @Dispatch()
  fetchIfaAndProceed() {
    return new GetAnonymousTokens();
  }

  ngOnInit() {
    this.actions$
      .pipe(ofActionDispatched(GetAnonymousTokensSuccess), takeUntil(this.isDestroyed))
      .subscribe(() => this.confirm());
    this.eventsTrackingService.trackEvent(EventTypes.bannerRequest);
  }

  ngOnDestroy() {
    this.isDestroyed.next();
  }

  closeWindow() {
    this.dialogWindowService.closeDialogWindow(true);
    this.eventsTrackingService.trackEvent(EventTypes.consentNotGranted);
  }

  private confirm() {
    this.authService.setTokenToOpener();
    this.eventsTrackingService.trackEvent(EventTypes.consentGranted);
    this.dialogWindowService.closeDialogWindow();
  }
}
