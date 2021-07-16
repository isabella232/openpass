import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IdentificationComponent } from './identification.component';
import { ViewContainerModule } from '@directives/view-container.module';

@NgModule({
  declarations: [IdentificationComponent],
  imports: [CommonModule, ViewContainerModule],
  exports: [IdentificationComponent],
})
export class IdentificationModule {}
