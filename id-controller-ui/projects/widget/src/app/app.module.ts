import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { environment } from '../environments/environment';
import { createCustomElement } from '@angular/elements';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule],
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
