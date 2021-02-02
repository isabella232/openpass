import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'usrf-auth-view',
  templateUrl: './auth-view.component.html',
  styleUrls: ['./auth-view.component.scss'],
})
export class AuthViewComponent implements OnInit {
  @Output() proceed = new EventEmitter<void>();

  websiteName = 'WebsiteName';
  toBeVerified = false;

  userEmail: string;
  verificationCode: string;
  allowToShareEmail = false;

  constructor() {}

  ngOnInit() {}

  submitForm() {
    if (!this.toBeVerified) {
      this.checkEmail();
    } else {
      this.checkVerification();
    }
  }

  private checkEmail() {
    // TODO: sendEmail to the server
    console.log('submitForm');
    this.toBeVerified = true;
  }

  private checkVerification() {
    // TODO: check verification
    this.proceed.emit();
  }
}
