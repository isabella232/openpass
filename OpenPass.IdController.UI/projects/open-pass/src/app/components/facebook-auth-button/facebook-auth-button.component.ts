import { Component, EventEmitter, Inject, OnDestroy, OnInit, Output } from '@angular/core';
import { WINDOW } from '@utils/injection-tokens';
import { environment } from '@env';

@Component({
  selector: 'usrf-facebook-auth-button',
  templateUrl: './facebook-auth-button.component.html',
  styleUrls: ['./facebook-auth-button.component.scss'],
})
export class FacebookAuthButtonComponent implements OnInit, OnDestroy {
  @Output()
  proceed = new EventEmitter<string>();

  // eslint-disable-next-line @typescript-eslint/naming-convention
  constructor(@Inject(WINDOW) private window: Window & { FB: any }) {}

  ngOnInit() {
    // @ts-ignore
    // eslint-disable-next-line no-underscore-dangle
    this.window._onSignIn = this.onSignIn.bind(this);
    this.window.FB.init(environment.facebookConfig);
  }

  ngOnDestroy() {
    // @ts-ignore
    // eslint-disable-next-line no-underscore-dangle
    delete this.window._onSignIn;
  }

  onSignIn() {
    this.window.FB.api('/me', { fields: 'email' }, ({ email }: { email: string }) => this.proceed.emit(email));
  }
}
