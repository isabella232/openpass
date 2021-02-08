import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OtpWidgetComponent } from './otp-widget.component';
import { OtpWidgetRoutingModule } from './otp-widget-routing.module';

@NgModule({
  imports: [CommonModule, OtpWidgetRoutingModule],
  declarations: [OtpWidgetComponent],
  exports: [OtpWidgetComponent],
})
export class OtpWidgetModule {}
