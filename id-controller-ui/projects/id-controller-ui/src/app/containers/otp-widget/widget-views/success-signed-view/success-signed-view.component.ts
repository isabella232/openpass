import { Component, Inject, OnInit } from '@angular/core';
import { localStorage } from '@utils/storage-decorator';
import { timer } from 'rxjs';
import { AuthService } from '@services/auth.service';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'usrf-success-signed-view',
  templateUrl: './success-signed-view.component.html',
  styleUrls: ['./success-signed-view.component.scss'],
})
export class SuccessSignedViewComponent implements OnInit {
  @localStorage('openpass.email')
  private userEmail: string;

  get secureEmail() {
    const visibleCharsCount = 3;
    const hiddenChars = Math.min(this.userEmail?.length - visibleCharsCount, 10);
    return this.userEmail?.slice(0, visibleCharsCount) + '*'.repeat(hiddenChars);
  }

  constructor(
    @Inject('Window') private window: Window,
    @Inject(DOCUMENT) private document: Document,
    private authService: AuthService
  ) {}

  ngOnInit() {
    // TODO: remove fake cookie setup
    this.document.cookie = 'OP_token=fake-token';

    this.authService.setTokenToOpener();
    if (this.window.opener) {
      timer(5000).subscribe(() => this.window.close());
    }
  }
}
