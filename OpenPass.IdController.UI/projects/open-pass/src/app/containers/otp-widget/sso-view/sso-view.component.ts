import { Component, OnInit } from '@angular/core';
import { Actions, ofActionDispatched, Select } from '@ngxs/store';
import { SsoState } from '@store/otp-widget/sso.state';
import { Observable } from 'rxjs';
import { EventTypes } from '@shared/enums/event-types.enum';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { EventsService } from '@rest/events/events.service';
import { GetTokenByEmail, ReceiveToken } from '@store/otp-widget/auth.actions';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { AuthState, IAuthState } from '@store/otp-widget/auth.state';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'usrf-sso-view',
  templateUrl: './sso-view.component.html',
  styleUrls: ['./sso-view.component.scss'],
})
export class SsoViewComponent implements OnInit {
  @Select(SsoState.isFetching)
  isFetching$: Observable<boolean>;
  @Select(AuthState.fullState)
  authState$: Observable<IAuthState>;

  eventTypes = EventTypes;

  constructor(
    private actions$: Actions,
    private authService: AuthService,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsService
  ) {}

  @Dispatch()
  getToken(email: string, eventType: EventTypes) {
    return new GetTokenByEmail(email, eventType);
  }

  ngOnInit() {
    this.actions$
      .pipe(ofActionDispatched(ReceiveToken), untilDestroyed(this))
      .subscribe(() => this.saveTokenAndClose());
  }

  private saveTokenAndClose() {
    this.authService.setTokenToOpener();
    this.eventsTrackingService
      .trackEvent(EventTypes.consentGranted)
      .pipe(untilDestroyed(this))
      .subscribe(() => this.dialogWindowService.closeDialogWindow());
  }
}
