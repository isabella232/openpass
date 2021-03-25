import { Inject, Injectable, NgZone } from '@angular/core';
import { WINDOW } from '@utils/injection-tokens';
import { environment } from '@env';

type WindowWithGapi = Window & { gapi: any };

@Injectable({
  providedIn: 'root',
})
export class GapiService {
  private get authInstance() {
    return this.window.gapi.auth2.getAuthInstance();
  }

  get isSignedIn(): boolean {
    return this.authInstance.isSignedIn.get();
  }

  get userEmail(): string {
    return this.authInstance.currentUser.get().getBasicProfile()?.getEmail();
  }

  constructor(@Inject(WINDOW) private window: WindowWithGapi) {}

  async load() {
    await new Promise((resolve) => this.window.gapi.load('auth2', resolve));
    // eslint-disable-next-line @typescript-eslint/naming-convention
    return this.window.gapi.auth2.init({ client_id: environment.googleClientId });
  }

  renderButton(element: HTMLElement) {
    this.window.gapi.signin2.render(element, { width: 'auto' });
  }

  subscribeToSignInEvent(callback: (isSignedIn: boolean) => void): { remove: () => void } {
    return this.authInstance.isSignedIn.listen((isSignedIn: boolean) => callback(isSignedIn));
  }

  signOut() {
    this.authInstance.signOut();
  }
}
