import { Component } from '@angular/core';
import { PostMessagePayload } from '@shared/types/post-message-payload';
import { PostMessageActions } from '@shared/enums/post-message-actions.enum';
import { AuthService } from '@services/auth.service';
import { PostMessagesService } from '@services/post-messages.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { EventsService } from '@rest/events/events.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'usrf-recognized-view',
  templateUrl: './recognized-view.component.html',
  styleUrls: ['./recognized-view.component.scss'],
})
export class RecognizedViewComponent {
  constructor(
    private authService: AuthService,
    private postMessagesService: PostMessagesService,
    private eventsTrackingService: EventsService
  ) {}

  continue() {
    this.authService.setTokenToOpener();
    this.eventsTrackingService
      .trackEvent(EventTypes.consentGranted)
      .pipe(untilDestroyed(this))
      .subscribe(() => this.postMessagesService.sendMessage({ action: PostMessageActions.closeChild }));
  }
}
