import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GoogleAuthButtonComponent } from './google-auth-button.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [GoogleAuthButtonComponent],
  exports: [GoogleAuthButtonComponent],
  imports: [CommonModule, TranslateModule],
})
export class GoogleAuthButtonModule {}
