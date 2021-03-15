import { Inject, Injectable } from '@angular/core';
import { WINDOW } from '@utils/injection-tokens';
import { environment } from '@env';
import { ReplaySubject } from 'rxjs';

type WindowWithGapi = Window & { gapi: any };

@Injectable({
  providedIn: 'root',
})
export class GapiService {
  gapi$ = new ReplaySubject();

  constructor(@Inject(WINDOW) private window: WindowWithGapi) {}

  load() {
    this.window.gapi.load('auth2', () => this.init());
  }

  init() {
    this.window.gapi.auth2
      .init({
        // eslint-disable-next-line @typescript-eslint/naming-convention
        client_id: environment.googleClientId,
      })
      .then(() => this.gapi$.next(this.window.gapi));
  }

  renderButton(element: HTMLElement) {
    this.gapi$.subscribe((gapi: any) => gapi.signin2.render(element));
  }
}
