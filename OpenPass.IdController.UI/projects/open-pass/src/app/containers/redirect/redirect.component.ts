import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Subscription, timer } from 'rxjs';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { GetTokenByEmail, ReceiveToken } from '@store/otp-widget/auth.actions';
import { EventTypes } from '@shared/enums/event-types.enum';
import { Actions, ofActionDispatched } from '@ngxs/store';
import { EventsTrackingService } from '@services/events-tracking.service';
import { WINDOW } from '@utils/injection-tokens';
import { AuthService } from '@services/auth.service';

@Component({
  selector: 'usrf-redirect',
  templateUrl: './redirect.component.html',
  styleUrls: ['./redirect.component.scss'],
})
export class RedirectComponent implements OnInit, OnDestroy {
  progress$ = new BehaviorSubject(30);

  redirectDomain: string;
  private redirectUrl: string;
  private userEmail: string;
  private authSubscriptions: Subscription;

  constructor(
    @Inject(WINDOW) private window: Window,
    private route: ActivatedRoute,
    private actions$: Actions,
    private authService: AuthService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  @Dispatch()
  proceedEmail(email: string) {
    this.userEmail = email;
    return new GetTokenByEmail(email, EventTypes.openPassSso);
  }

  ngOnInit() {
    const queryParams = this.route.snapshot.queryParams;

    timer(100).subscribe(() => this.progress$.next(90));
    this.authSubscriptions = this.actions$
      .pipe(ofActionDispatched(ReceiveToken))
      .subscribe(() => this.saveTokenAndClose());

    if (queryParams.origin) {
      this.parseDomain(queryParams.origin);
    }
    if (queryParams.email) {
      this.proceedEmail(queryParams.email);
    }
  }

  ngOnDestroy() {
    this.authSubscriptions?.unsubscribe();
  }

  private parseDomain(redirectUrl: string) {
    this.redirectUrl = redirectUrl;
    this.redirectDomain = new URL(redirectUrl).host;
  }

  private saveTokenAndClose() {
    this.eventsTrackingService.trackEvent(EventTypes.consentGranted);

    const originPath = new URL(this.redirectUrl);
    originPath.searchParams.set('email', this.userEmail ?? '');
    originPath.searchParams.set('ifaToken', this.authService.ifaToken ?? '');
    originPath.searchParams.set('uid2Token', this.authService.uid2Token ?? '');
    this.window.location.replace(originPath.toString());
  }
}
