import { AfterViewInit, Component, ElementRef, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { GapiService } from '@services/gapi.service';
import { AuthenticatedService } from '@rest/authenticated/authenticated.service';

@Component({
  selector: 'usrf-sso-view',
  templateUrl: './sso-view.component.html',
  styleUrls: ['./sso-view.component.scss'],
})
export class SsoViewComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('googleButton')
  googleButton: ElementRef;

  listener: any;

  isSignedIn = false;
  userEmail: string;

  constructor(
    private gapiService: GapiService,
    private zone: NgZone,
    private authenticatedService: AuthenticatedService
  ) {}

  ngOnInit(): void {
    this.gapiService.load().then(() => {
      this.refreshUserData();
      this.listener = this.gapiService.subscribeToSignInEvent((isSignedIn) => this.refreshUserData(isSignedIn));
    });
  }

  ngAfterViewInit() {
    this.gapiService.renderButton(this.googleButton.nativeElement);
  }

  ngOnDestroy() {
    this.listener?.remove?.();
  }

  signOut() {
    this.gapiService.signOut();
  }

  signIn() {
    this.authenticatedService.getTokenByEmail(this.gapiService.userEmail).subscribe(console.log);
  }

  private refreshUserData(isSignedIn = false) {
    this.zone.run(() => {
      this.isSignedIn = this.gapiService.isSignedIn;
      this.userEmail = this.gapiService.userEmail;
    });

    if (!isSignedIn && this.googleButton) {
      this.gapiService.renderButton(this.googleButton.nativeElement);
    }

    if (isSignedIn) {
      this.signIn();
    }
  }
}
