import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FacebookAuthComponent } from './facebook-auth.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [FacebookAuthComponent],
  exports: [FacebookAuthComponent],
  imports: [CommonModule, TranslateModule],
})
export class FacebookAuthModule {}
