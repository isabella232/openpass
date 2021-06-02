import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GoogleAuthComponent } from './google-auth.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [GoogleAuthComponent],
  exports: [GoogleAuthComponent],
  imports: [CommonModule, TranslateModule],
})
export class GoogleAuthModule {}
