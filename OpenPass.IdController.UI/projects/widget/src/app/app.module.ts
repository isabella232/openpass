import { ApplicationRef, DoBootstrap, Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { createCustomElement } from '@angular/elements';
import { windowFactory } from '@utils/window-factory';
import { deployUrl } from '@utils/deploy-url-factory';
import { DEPLOY_URL, WINDOW } from '@utils/injection-tokens';
import { IdentificationComponent } from './containers/identification/identification.component';
import { TranslationModule } from './containers/shared/translation.module';
import { LandingComponent } from './containers/landing/landing.component';
import { SnackBarComponent } from '@components/snack-bar/snack-bar.component';
import { ViewContainerModule } from '@directives/view-container.module';
import { TokensCatcherModule } from './containers/tokens-catcher/tokens-catcher.module';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, HttpClientModule, TranslationModule, ViewContainerModule, TokensCatcherModule],
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
      'wdgt-landing': LandingComponent,
      'wdgt-snack-bar': SnackBarComponent,
    };

    Object.entries(componentsMap).forEach(([tagName, component]) => {
      const customComponent = createCustomElement(component, { injector: this.injector });
      customElements.define(tagName, customComponent);
    });
  }
}
