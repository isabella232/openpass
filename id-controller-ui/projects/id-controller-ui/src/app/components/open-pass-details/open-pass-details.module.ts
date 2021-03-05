import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OpenPassDetailsComponent } from './open-pass-details.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [OpenPassDetailsComponent],
  imports: [CommonModule, TranslateModule],
  exports: [OpenPassDetailsComponent],
})
export class OpenPassDetailsModule {}
