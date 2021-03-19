import { Component, OnDestroy, OnInit } from '@angular/core';
import { Actions, ofActionDispatched, Select, Store } from '@ngxs/store';
import { OpenerState } from '@store/otp-widget/opener.state';
import { Observable, Subscription } from 'rxjs';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { AuthState, IAuthState } from '@store/otp-widget/auth.state';
import {
  GenerateCode,
  SetCode,
  SetEmail,
  SetShareEmailValue,
  ValidateCode,
  ValidateCodeSuccess,
} from '@store/otp-widget/auth.actions';
import { EventTypes } from '@enums/event-types.enum';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { EventsTrackingService } from '@services/events-tracking.service';

@Component({
  selector: 'usrf-auth-view',
  templateUrl: './auth-view.component.html',
  styleUrls: ['./auth-view.component.scss'],
})
export class AuthViewComponent implements OnInit, OnDestroy {
  @Select(OpenerState.originFormatted) websiteName$: Observable<string>;
  @Select(AuthState.fullState) authState$: Observable<IAuthState>;

  private authSubscriptions: Subscription;

  constructor(
    private store: Store,
    private actions$: Actions,
    private authService: AuthService,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  @Dispatch()
  submitForm() {
    const { isEmailVerified } = this.store.selectSnapshot(AuthState.fullState);
    return isEmailVerified ? new ValidateCode() : new GenerateCode();
  }

  @Dispatch()
  patchEmail({ target }: Event) {
    return new SetEmail((target as HTMLInputElement).value);
  }

  @Dispatch()
  patchCode({ target }: Event) {
    return new SetCode((target as HTMLInputElement).value);
  }

  @Dispatch()
  patchShareEmail({ target }: Event) {
    return new SetShareEmailValue((target as HTMLInputElement).checked);
  }

  ngOnInit() {
    this.authSubscriptions = this.actions$
      .pipe(ofActionDispatched(ValidateCodeSuccess))
      .subscribe(() => this.saveTokenAndClose());
  }

  ngOnDestroy() {
    this.authSubscriptions?.unsubscribe?.();
  }

  private saveTokenAndClose() {
    this.authService.setTokenToOpener();
    this.eventsTrackingService.trackEvent(EventTypes.consentGranted);
    this.dialogWindowService.closeDialogWindow();
  }
}
