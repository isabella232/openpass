import {
  Component,
  ElementRef,
  OnInit,
  Output,
  ViewChild,
  EventEmitter,
  OnDestroy,
  AfterViewInit,
} from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { SsoState } from '@store/otp-widget/sso.state';
import { Observable } from 'rxjs';
import { ReadUserData, SetFetchGoogleApi } from '@store/otp-widget/sso.actions';
import { GapiService } from '@services/gapi.service';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';

@Component({
  selector: 'usrf-google-auth-button',
  templateUrl: './google-auth-button.component.html',
  styleUrls: ['./google-auth-button.component.scss'],
})
export class GoogleAuthButtonComponent implements OnInit, OnDestroy, AfterViewInit {
  @Output()
  proceed = new EventEmitter<string>();

  @ViewChild('googleButton')
  googleButton: ElementRef;

  @Select(SsoState.isSignedIn)
  isSignedIn$: Observable<boolean>;
  @Select(SsoState.userEmail)
  userEmail$: Observable<string>;
  @Select(SsoState.isFetching)
  isFetching$: Observable<boolean>;

  private listener: { remove: () => void };

  constructor(private store: Store, private gapiService: GapiService) {}

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
  }

  ngAfterViewInit() {
    if (!this.store.selectSnapshot(SsoState.isSignedIn) && this.googleButton?.nativeElement) {
      this.gapiService.renderButton(this.googleButton.nativeElement);
    }
  }

  ngOnDestroy() {
    this.listener?.remove?.();
  }

  signOut() {
    this.gapiService.signOut();
  }

  continueWith() {
    this.proceed.emit(this.gapiService.userEmail);
  }

  private syncGoogleButton(isSignedIn = false) {
    this.refreshUserData();

    if (!isSignedIn && this.googleButton) {
      this.gapiService.renderButton(this.googleButton.nativeElement);
    }
    if (isSignedIn) {
      this.proceed.emit(this.gapiService.userEmail);
    }
  }
}
