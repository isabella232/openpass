import { AfterViewInit, Component, ElementRef, EventEmitter, Output, ViewChild } from '@angular/core';
import { Store } from '@ngxs/store';
import { GapiService } from '@services/gapi.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'usrf-google-auth',
  templateUrl: './google-auth.component.html',
  styleUrls: ['./google-auth.component.scss'],
})
export class GoogleAuthComponent implements AfterViewInit {
  @Output()
  proceed = new EventEmitter<string>();

  @ViewChild('googleButton')
  googleButton: ElementRef;

  constructor(private store: Store, private gapiService: GapiService) {}

  ngAfterViewInit() {
    this.gapiService.load().then(() => {
      this.gapiService
        .attachCustomButton(this.googleButton.nativeElement)
        .pipe(untilDestroyed(this))
        .subscribe(() => this.onSignIn());
    });
  }

  onSignIn() {
    this.proceed.emit(this.gapiService.userEmail);
  }
}
