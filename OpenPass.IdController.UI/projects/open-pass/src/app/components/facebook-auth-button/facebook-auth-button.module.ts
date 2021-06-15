import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FacebookAuthButtonComponent } from './facebook-auth-button.component';

@NgModule({
  declarations: [FacebookAuthButtonComponent],
  exports: [FacebookAuthButtonComponent],
  imports: [CommonModule],
})
export class FacebookAuthButtonModule {}
