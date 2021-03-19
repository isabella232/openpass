import { Inject, Injectable } from '@angular/core';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { WINDOW } from '@utils/injection-tokens';
import { PostMessagesService } from '@services/post-messages.service';

@Injectable({
  providedIn: 'root',
})
export class DialogWindowService {
  constructor(@Inject(WINDOW) private window: Window, private postMessagesService: PostMessagesService) {}

  closeDialogWindow() {
    const message: PostMessagePayload = { action: PostMessageActions.closeChild };
    this.postMessagesService.sendMessage(message);
    this.window.close(); // fallback
  }
}
