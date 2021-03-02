import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgxsModule } from '@ngxs/store';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { windowFactory } from '@utils/window-factory';
import { environment } from '@env';
import { NgxsDispatchPluginModule } from '@ngxs-labs/dispatch-decorator';
import { AuthState } from '@store/otp-widget/auth.state';
import { OpenerState } from '@store/otp-widget/opener.state';
import { OtpWidgetState } from '@store/otp-widget/otp-widget.state';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';

export const createTranslateLoader = (http: HttpClient): TranslateHttpLoader =>
  new TranslateHttpLoader(http, './assets/i18n/', '.json');

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    RouterModule,
    AppRoutingModule,
    HttpClientModule,
    NgxsModule.forRoot([OpenerState, OtpWidgetState, AuthState], {
      developmentMode: !environment.production,
    }),
    NgxsDispatchPluginModule.forRoot(),
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
    }),
  ],
  providers: [{ provide: 'Window', useFactory: windowFactory }],
  bootstrap: [AppComponent],
})
export class AppModule {}
