import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { OpenPassDetailsComponent } from './open-pass-details.component';
import { PipesModule } from '../../pipes/pipes.module';

@NgModule({
  declarations: [OpenPassDetailsComponent],
  imports: [CommonModule, TranslateModule, PipesModule],
  exports: [OpenPassDetailsComponent],
})
export class OpenPassDetailsModule {}
