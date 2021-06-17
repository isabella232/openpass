import { Inject, Injectable } from '@angular/core';
import { WINDOW } from '@utils/injection-tokens';
import { environment } from '@env';
import { ReplaySubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { fromPromise } from 'rxjs/internal-compatibility';

type WindowWithGapi = Window & { gapi: any };

@Injectable({
  providedIn: 'root',
})
export class GapiService {
  private gapiStateLoaded = new ReplaySubject<boolean>();

  private get authInstance() {
    return this.window.gapi.auth2.getAuthInstance();
  }

  get userEmail(): string {
    return this.authInstance.currentUser.get().getBasicProfile()?.getEmail();
  }

  constructor(@Inject(WINDOW) private window: WindowWithGapi) {}

  async load() {
    await new Promise((resolve) => this.window.gapi.load('auth2', resolve));
    // eslint-disable-next-line @typescript-eslint/naming-convention
    const auth = await this.window.gapi.auth2.init({ client_id: environment.googleClientId });
    this.gapiStateLoaded.next(true);
    return auth;
  }

  signOut() {
    this.authInstance.signOut();
  }

  attachCustomButton(element: HTMLButtonElement) {
    const signInEvent = new Promise((resolve) => {
      this.gapiStateLoaded.pipe(take(1)).subscribe(() => this.authInstance.attachClickHandler(element, {}, resolve));
    });
    return fromPromise(signInEvent);
  }
}
