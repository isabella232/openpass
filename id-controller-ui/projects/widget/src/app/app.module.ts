import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { environment } from '../environments/environment';
import { createCustomElement } from '@angular/elements';
import { OtpWidgetModule } from './containers/otp-widget/otp-widget.module';
import { windowFactory } from './utils/window-factory';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, OtpWidgetModule],
  providers: [{ provide: 'Window', useFactory: windowFactory }],
  bootstrap: environment.production ? [] : [AppComponent],
})
export class AppModule implements DoBootstrap {
  constructor(private injector: Injector) {
    if (environment.production) {
      const webElement = createCustomElement(AppComponent, { injector });
      customElements.define('wdgt-identification', webElement);
    }
  }

  ngDoBootstrap(appRef: ApplicationRef) {}
}
