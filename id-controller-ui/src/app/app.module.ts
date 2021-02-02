import { BrowserModule } from '@angular/platform-browser';
import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { environment } from '@env';

import { AppComponent } from './app.component';
import { OtpWidgetModule } from './containers/otp-widget/otp-widget.module';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, OtpWidgetModule],
  providers: [],
  bootstrap: environment.production ? [] : [AppComponent],
})
export class AppModule implements DoBootstrap {
  constructor(private injector: Injector) {
    if (environment.production) {
      const webElement = createCustomElement(AppComponent, { injector });
      customElements.define('usrf-identification', webElement);
    }
  }

  ngDoBootstrap(appRef: ApplicationRef) {}
}
