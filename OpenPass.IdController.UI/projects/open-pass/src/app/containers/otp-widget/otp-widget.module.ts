import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OtpWidgetComponent } from './otp-widget.component';
import { OtpWidgetRoutingModule } from './otp-widget-routing.module';
import { NavigationModule } from '../../components/navigation/navigation.module';

@NgModule({
  imports: [CommonModule, OtpWidgetRoutingModule, NavigationModule],
  declarations: [OtpWidgetComponent],
  exports: [OtpWidgetComponent],
})
export class OtpWidgetModule {}
