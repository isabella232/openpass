import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CopyrightComponent } from './copyright/copyright.component';

@NgModule({
  declarations: [CopyrightComponent],
  exports: [CopyrightComponent],
  imports: [CommonModule],
})
export class SharedModule {}
