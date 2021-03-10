import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CopyrightComponent } from './copyright/copyright.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [CopyrightComponent],
  exports: [CopyrightComponent],
  imports: [CommonModule, TranslateModule],
})
export class SharedModule {}
