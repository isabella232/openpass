import { Inject, Injectable } from '@angular/core';
import { WINDOW } from '../utils/injection-tokens';

@Injectable({
  providedIn: 'root',
})
export class EnvironmentService {
  constructor(@Inject(WINDOW) private window: Window) {}

  get isProd() {
    return this.window.location.host.match(/.criteo\.com$/);
  }

  get isPreprod() {
    return this.window.location.host.match(/\.preprod/);
  }

  get isLocal() {
    return !(this.isPreprod || this.isProd);
  }
}
