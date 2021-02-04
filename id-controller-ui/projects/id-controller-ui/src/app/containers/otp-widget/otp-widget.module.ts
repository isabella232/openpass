import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OtpWidgetComponent } from './otp-widget.component';
import { WidgetViewsModule } from './widget-views/widget-views.module';

@NgModule({
  imports: [CommonModule, WidgetViewsModule],
  declarations: [OtpWidgetComponent],
  exports: [OtpWidgetComponent],
})
export class OtpWidgetModule {}
