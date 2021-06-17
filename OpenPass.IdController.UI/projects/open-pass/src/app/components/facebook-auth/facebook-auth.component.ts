import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { environment } from '@env';
import { WINDOW } from '@utils/injection-tokens';

@Component({
  selector: 'usrf-facebook-auth',
  templateUrl: './facebook-auth.component.html',
  styleUrls: ['./facebook-auth.component.scss'],
})
export class FacebookAuthComponent implements OnInit {
  @Output()
  proceed = new EventEmitter<string>();

  // eslint-disable-next-line @typescript-eslint/naming-convention
  constructor(@Inject(WINDOW) private window: Window & { FB: any }) {}

  ngOnInit() {
    this.window.FB.init(environment.facebookConfig);
  }

  launchFacebookDialog() {
    this.window.FB.login((response: Record<string, unknown>) => {
      if (response.status === 'connected') {
        this.onSignIn();
      }
    });
  }

  private onSignIn() {
    this.window.FB.api('/me', { fields: 'email' }, ({ email }: { email: string }) => this.proceed.emit(email));
  }
}
