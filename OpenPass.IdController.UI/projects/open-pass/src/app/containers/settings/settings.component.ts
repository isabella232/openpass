import { Component } from '@angular/core';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { Select } from '@ngxs/store';
import { AuthState, IAuthState } from '@store/otp-widget/auth.state';
import { Observable } from 'rxjs';
import { localStorage } from '@shared/utils/storage-decorator';
import { PerformOptOut } from '@store/controls.actions';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { PostMessagesService } from '@services/post-messages.service';

@Component({
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent {
  @Select(AuthState.fullState) authState$: Observable<IAuthState>;

  @localStorage('openpass.email')
  readonly userEmail: string;

  get secureEmail(): string {
    if (!this.userEmail) {
      return undefined;
    }
    const [mailName, mailHost] = this.userEmail.split('@');
    const hiddenCharCount = mailName.length - 2;
    return `${mailName[0]}${'*'.repeat(hiddenCharCount)}${mailName[mailName.length - 1]}@${mailHost}`;
  }

  constructor(private postMessageService: PostMessagesService) {}

  @Dispatch()
  optOut(): PerformOptOut {
    this.postMessageService.sendMessage({ action: PostMessageActions.optOut });

    return new PerformOptOut();
  }
}
