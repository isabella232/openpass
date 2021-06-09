import { Component, Inject, OnInit } from '@angular/core';
import { localStorage } from '@shared/utils/storage-decorator';
import { AuthService } from '@services/auth.service';
import { Router } from '@angular/router';
import { WINDOW } from '@utils/injection-tokens';
import { DialogWindowService } from '@services/dialog-window.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { EventsTrackingService } from '@services/events-tracking.service';

@Component({
  selector: 'usrf-already-signed-view',
  templateUrl: './already-signed-view.component.html',
  styleUrls: ['./already-signed-view.component.scss'],
})
export class AlreadySignedViewComponent implements OnInit {
  @localStorage('openpass.email')
  private storageUserEmail: string;

  userEmail: string;

  constructor(
    private authService: AuthService,
    @Inject(WINDOW) private window: Window,
    private router: Router,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  ngOnInit() {
    this.userEmail = this.storageUserEmail;
  }

  submitForm() {
    if (this.userEmail !== this.storageUserEmail) {
      this.storageUserEmail = this.userEmail;
    }
    this.authService.setTokenToOpener();
    this.eventsTrackingService.trackEvent(EventTypes.consentGranted);
    this.dialogWindowService.closeDialogWindow();
  }

  resetState() {
    this.storageUserEmail = '';
    this.userEmail = '';
    this.authService.resetToken();
    this.router.navigate(['auth']);
  }
}
