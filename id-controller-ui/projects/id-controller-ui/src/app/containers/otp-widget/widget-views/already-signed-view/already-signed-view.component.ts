import { Component, Inject, OnInit } from '@angular/core';
import { localStorage } from '@utils/storage-decorator';
import { AuthService } from '@services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'usrf-already-signed-view',
  templateUrl: './already-signed-view.component.html',
  styleUrls: ['./already-signed-view.component.scss'],
})
export class AlreadySignedViewComponent implements OnInit {
  @localStorage('openpass.email')
  private storageUserEmail: string;

  userEmail: string;

  constructor(private authService: AuthService, @Inject('Window') private window: Window, private router: Router) {}

  ngOnInit() {
    this.userEmail = this.storageUserEmail;
  }

  submitForm() {
    if (this.userEmail !== this.storageUserEmail) {
      this.storageUserEmail = this.userEmail;
    }
    this.router.navigate(['success']);
  }

  resetState() {
    this.storageUserEmail = '';
    this.userEmail = '';
    this.authService.resetToken();
    this.router.navigate(['auth']);
  }
}
