import { Component } from '@angular/core';
import { localStorage } from '@utils/storage-decorator';

@Component({
  selector: 'usrf-success-signed-view',
  templateUrl: './success-signed-view.component.html',
  styleUrls: ['./success-signed-view.component.scss'],
})
export class SuccessSignedViewComponent {
  @localStorage('crto.email')
  private userEmail: string;

  get secureEmail() {
    const visibleCharsCount = 3;
    const hiddenChars = Math.min(this.userEmail?.length - visibleCharsCount, 10);
    return this.userEmail?.slice(0, visibleCharsCount) + '*'.repeat(hiddenChars);
  }
}
