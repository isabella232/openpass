import { ApplicationRef, DoBootstrap, Inject, Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { environment } from '../environments/environment';
import { createCustomElement } from '@angular/elements';
import { ViewContainerDirective } from './directives/view-container.directive';
import { windowFactory } from './utils/window-factory';
import { deployUrl } from './utils/deploy-url-factory';
import { DEPLOY_URL, WINDOW } from './utils/injection-tokens';

@NgModule({
  declarations: [AppComponent, ViewContainerDirective],
  imports: [BrowserModule],
  providers: [
    { provide: WINDOW, useFactory: windowFactory },
    { provide: DEPLOY_URL, useFactory: deployUrl },
  ],
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
