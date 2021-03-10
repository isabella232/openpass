import { Component } from '@angular/core';
import { Select } from '@ngxs/store';
import { OpenerState } from '@store/otp-widget/opener.state';
import { Observable } from 'rxjs';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { AuthService } from '@services/auth.service';
import { PostMessagesService } from '@services/post-messages.service';

@Component({
  selector: 'usrf-agreement-view',
  templateUrl: './agreement-view.component.html',
  styleUrls: ['./agreement-view.component.scss'],
})
export class AgreementViewComponent {
  @Select(OpenerState.originFormatted)
  websiteName$: Observable<string>;

  constructor(private authService: AuthService, private postMessagesService: PostMessagesService) {}

  confirm() {
    const message: PostMessagePayload = { action: PostMessageActions.closeChild };
    this.authService.setTokenToOpener();
    this.postMessagesService.sendMessage(message);
  }
}
