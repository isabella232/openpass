import { Inject, Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Store } from '@ngxs/store';
import { OpenerState } from '@store/otp-widget/opener.state';
import { WINDOW } from '@utils/injection-tokens';

@Injectable()
export class OriginInterceptor implements HttpInterceptor {
  constructor(private store: Store, @Inject(WINDOW) private window: Window) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const widgetConfig = { ...this.store.selectSnapshot(OpenerState.fullState) };
    widgetConfig.origin ??= this.window.location.origin;
    const patch = { setHeaders: { 'x-tracked-data': JSON.stringify(widgetConfig) } };
    return next.handle(request.clone(patch));
  }
}
