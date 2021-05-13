import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FacebookAuthSmallComponent } from './facebook-auth-small.component';

@NgModule({
  declarations: [FacebookAuthSmallComponent],
  exports: [FacebookAuthSmallComponent],
  imports: [CommonModule],
})
export class FacebookAuthSmallModule {}
