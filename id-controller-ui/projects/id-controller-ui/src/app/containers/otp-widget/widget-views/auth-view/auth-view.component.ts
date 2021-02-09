import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { localStorage } from '@utils/storage-decorator';

@Component({
  selector: 'usrf-auth-view',
  templateUrl: './auth-view.component.html',
  styleUrls: ['./auth-view.component.scss'],
})
export class AuthViewComponent implements OnInit {
  @localStorage('crto.email')
  private storageUserEmail: string;

  websiteName = 'WebsiteName';
  toBeVerified = false;

  userEmail: string;
  verificationCode: string;
  allowToShareEmail = false;

  constructor(private router: Router) {}

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
    this.storageUserEmail = this.userEmail;
    this.router.navigate(['agreement']);
  }
}
