import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { GapiService } from '@services/gapi.service';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { GetTokenByEmail, ValidateCodeSuccess } from '@store/otp-widget/auth.actions';
import { Actions, ofActionDispatched, Select, Store } from '@ngxs/store';
import { SsoState } from '@store/otp-widget/sso.state';
import { Observable, Subscription } from 'rxjs';
import { ReadUserData, SetFetchGoogleApi } from '@store/otp-widget/sso.actions';
import { EventTypes } from '@enums/event-types.enum';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';
import { EventsTrackingService } from '@services/events-tracking.service';

@Component({
  selector: 'usrf-sso-view',
  templateUrl: './sso-view.component.html',
  styleUrls: ['./sso-view.component.scss'],
})
export class SsoViewComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('googleButton')
  googleButton: ElementRef;

  @Select(SsoState.isSignedIn)
  isSignedIn$: Observable<boolean>;
  @Select(SsoState.userEmail)
  userEmail$: Observable<string>;
  @Select(SsoState.isFetching)
  isFetching$: Observable<boolean>;

  private listener: any;
  private authSubscriptions: Subscription;

  constructor(
    private store: Store,
    private actions$: Actions,
    private gapiService: GapiService,
    private authService: AuthService,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  @Dispatch()
  getToken() {
    return new GetTokenByEmail(this.gapiService.userEmail);
  }

  @Dispatch()
  private refreshUserData() {
    return new ReadUserData(this.gapiService.userEmail, this.gapiService.isSignedIn);
  }

  ngOnInit(): void {
    this.store.dispatch(new SetFetchGoogleApi(true));
    this.gapiService.load().then(() => {
      this.syncGoogleButton();
      this.listener = this.gapiService.subscribeToSignInEvent((isSignedIn) => this.syncGoogleButton(isSignedIn));
    });
    this.authSubscriptions = this.actions$
      .pipe(ofActionDispatched(ValidateCodeSuccess))
      .subscribe(() => this.saveTokenAndClose());
  }

  ngAfterViewInit() {
    if (!this.store.selectSnapshot(SsoState.isSignedIn) && this.googleButton?.nativeElement) {
      this.gapiService.renderButton(this.googleButton.nativeElement);
    }
  }

  ngOnDestroy() {
    this.listener?.remove?.();
    this.authSubscriptions?.unsubscribe?.();
  }

  signOut() {
    this.gapiService.signOut();
  }

  private syncGoogleButton(isSignedIn = false) {
    this.refreshUserData();

    if (!isSignedIn && this.googleButton) {
      this.gapiService.renderButton(this.googleButton.nativeElement);
    }
    if (isSignedIn) {
      this.getToken();
    }
  }

  private saveTokenAndClose() {
    this.authService.setTokenToOpener();
    this.eventsTrackingService.trackEvent(EventTypes.consentGranted);
    this.dialogWindowService.closeDialogWindow();
  }
}
