import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GoogleAuthSmallComponent } from './google-auth-small.component';

@NgModule({
  declarations: [GoogleAuthSmallComponent],
  exports: [GoogleAuthSmallComponent],
  imports: [CommonModule],
})
export class GoogleAuthSmallModule {}
