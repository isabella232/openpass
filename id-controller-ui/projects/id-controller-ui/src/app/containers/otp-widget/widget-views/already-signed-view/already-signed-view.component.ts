import { Component, OnInit } from '@angular/core';
import { localStorage } from '@utils/storage-decorator';
import { AuthService } from '@services/auth.service';

@Component({
  selector: 'usrf-already-signed-view',
  templateUrl: './already-signed-view.component.html',
  styleUrls: ['./already-signed-view.component.scss'],
})
export class AlreadySignedViewComponent implements OnInit {
  @localStorage('openpass.email')
  private storageUserEmail: string;

  userEmail: string;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.setTokenToOpener();
    this.userEmail = this.storageUserEmail;
  }

  submitForm() {
    if (this.userEmail !== this.storageUserEmail) {
      this.storageUserEmail = this.userEmail;
    }

    // TODO: proceed
  }
}
