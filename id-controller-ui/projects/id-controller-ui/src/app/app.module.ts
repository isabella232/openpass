import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { OtpWidgetModule } from './containers/otp-widget/otp-widget.module';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, OtpWidgetModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
