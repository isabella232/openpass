import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { OpenPassDetailsComponent } from './open-pass-details.component';

@NgModule({
  declarations: [OpenPassDetailsComponent],
  imports: [CommonModule, TranslateModule],
  exports: [OpenPassDetailsComponent],
})
export class OpenPassDetailsModule {}
