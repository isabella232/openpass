import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GoogleAuthButtonComponent } from './google-auth-button.component';

@NgModule({
  declarations: [GoogleAuthButtonComponent],
  exports: [GoogleAuthButtonComponent],
  imports: [CommonModule],
})
export class GoogleAuthButtonModule {}
