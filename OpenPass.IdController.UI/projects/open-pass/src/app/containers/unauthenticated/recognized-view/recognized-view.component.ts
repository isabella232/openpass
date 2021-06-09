import { Component } from '@angular/core';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { AuthService } from '@services/auth.service';
import { PostMessagesService } from '@services/post-messages.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { EventsTrackingService } from '@services/events-tracking.service';

@Component({
  selector: 'usrf-recognized-view',
  templateUrl: './recognized-view.component.html',
  styleUrls: ['./recognized-view.component.scss'],
})
export class RecognizedViewComponent {
  constructor(
    private authService: AuthService,
    private postMessagesService: PostMessagesService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  continue() {
    const message: PostMessagePayload = { action: PostMessageActions.closeChild };
    this.eventsTrackingService.trackEvent(EventTypes.consentGranted);
    this.authService.setTokenToOpener();
    this.postMessagesService.sendMessage(message);
  }
}
