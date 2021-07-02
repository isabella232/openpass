import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { createCustomElement } from '@angular/elements';
import { windowFactory } from './utils/window-factory';
import { deployUrl } from './utils/deploy-url-factory';
import { DEPLOY_URL, WINDOW } from './utils/injection-tokens';
import { IdentificationComponent } from './containers/identification/identification.component';
import { IdentificationModule } from './containers/identification/identification.module';
import { TranslationModule } from './containers/shared/translation.module';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, IdentificationModule, HttpClientModule, TranslationModule],
  providers: [
    { provide: WINDOW, useFactory: windowFactory },
    { provide: DEPLOY_URL, useFactory: deployUrl },
  ],
})
export class AppModule implements DoBootstrap {
  constructor(private injector: Injector) {}

  ngDoBootstrap(appRef: ApplicationRef) {
    const componentsMap = {
      'wdgt-app': AppComponent,
      'wdgt-identification': IdentificationComponent,
    };

    Object.entries(componentsMap).forEach(([tagName, component]) => {
      const customComponent = createCustomElement(component, { injector: this.injector });
      customElements.define(tagName, customComponent);
    });
  }
}
