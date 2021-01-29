import { BrowserModule } from '@angular/platform-browser';
import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { createCustomElement } from '@angular/elements';

import { AppComponent } from './app.component';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule implements DoBootstrap {
  constructor(private injector: Injector) {
    const webElement = createCustomElement(AppComponent, { injector });
    customElements.define('usrf-identification', webElement);
  }

  ngDoBootstrap(appRef: ApplicationRef) {}
}
