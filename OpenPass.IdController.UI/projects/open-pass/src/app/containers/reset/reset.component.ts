import { Component, Inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { DialogWindowService } from '../../services/dialog-window.service';
import { GapiService } from '../../services/gapi.service';
import { WINDOW } from '../../utils/injection-tokens';

@Component({
  selector: 'usrf-reset',
  template: '',
})
export class ResetComponent implements OnInit {
  constructor(
    private authService: AuthService,
    private dialogWindowService: DialogWindowService,
    private gapiService: GapiService,
    // eslint-disable-next-line @typescript-eslint/naming-convention
    @Inject(WINDOW) private window: Window & { FB: any }
  ) {}

  ngOnInit() {
    this.authService.resetToken();
    this.gapiService.load().then(() => this.gapiService.signOut());
    this.window.FB.logout();
    this.dialogWindowService.closeDialogWindow();
  }
}
