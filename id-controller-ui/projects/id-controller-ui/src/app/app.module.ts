import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgxsModule } from '@ngxs/store';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { windowFactory } from '@utils/window-factory';
import { environment } from '@env';
import { NgxsDispatchPluginModule } from '@ngxs-labs/dispatch-decorator';
import { AuthState } from '@store/otp-widget/auth.state';
import { OpenerState } from '@store/otp-widget/opener.state';
import { OtpWidgetState } from '@store/otp-widget/otp-widget.state';

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
  ],
  providers: [{ provide: 'Window', useFactory: windowFactory }],
  bootstrap: [AppComponent],
})
export class AppModule {}
