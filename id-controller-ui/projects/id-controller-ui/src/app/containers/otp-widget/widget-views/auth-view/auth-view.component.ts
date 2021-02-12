import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { localStorage } from '@utils/storage-decorator';
import { OtpService } from '@rest/otp/otp.service';
import { OtpDto } from '@rest/otp/otp.dto';

@Component({
  selector: 'usrf-auth-view',
  templateUrl: './auth-view.component.html',
  styleUrls: ['./auth-view.component.scss'],
})
export class AuthViewComponent {
  @localStorage('openpass.email')
  private storageUserEmail: string;

  isFetching = false;
  websiteName = 'WebsiteName';
  toBeVerified = false;

  userEmail: string;
  verificationCode: string;
  allowToShareEmail = false;
  codeVerificationFailed = false;
  emailVerificationFailed = false;

  constructor(private router: Router, private otpService: OtpService) {}

  submitForm() {
    if (!this.toBeVerified) {
      this.checkEmail();
    } else {
      this.checkVerification();
    }
  }

  private checkEmail() {
    this.isFetching = true;
    this.emailVerificationFailed = false;
    const otp = new OtpDto({ email: this.userEmail });
    this.otpService.generateOtp(otp).subscribe(
      () => (this.toBeVerified = true),
      () => {
        this.isFetching = false;
        this.emailVerificationFailed = true;
      },
      () => (this.isFetching = false)
    );
  }

  private checkVerification() {
    this.isFetching = true;
    this.codeVerificationFailed = false;
    const otp = new OtpDto({ email: this.userEmail, otp: this.verificationCode });
    this.otpService.validateOtp(otp).subscribe(
      () => {
        this.storageUserEmail = this.userEmail;
        this.router.navigate(['agreement'], { queryParamsHandling: 'preserve' });
      },
      () => {
        this.isFetching = false;
        this.codeVerificationFailed = true;
      },
      () => (this.isFetching = false)
    );
  }
}
