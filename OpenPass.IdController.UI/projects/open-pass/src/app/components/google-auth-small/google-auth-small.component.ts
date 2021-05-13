import { AfterViewInit, Component, ElementRef, EventEmitter, Output, ViewChild } from '@angular/core';
import { Store } from '@ngxs/store';
import { GapiService } from '@services/gapi.service';

@Component({
  selector: 'usrf-google-auth-small',
  templateUrl: './google-auth-small.component.html',
  styleUrls: ['./google-auth-small.component.scss'],
})
export class GoogleAuthSmallComponent implements AfterViewInit {
  @Output()
  proceed = new EventEmitter<string>();

  @ViewChild('googleButton')
  googleButton: ElementRef;

  constructor(private store: Store, private gapiService: GapiService) {}

  ngAfterViewInit() {
    this.gapiService.load().then(() => {
      this.gapiService.attachCustomButton(this.googleButton.nativeElement).subscribe(() => this.onSignIn());
    });
  }

  onSignIn() {
    this.proceed.emit(this.gapiService.userEmail);
  }
}
