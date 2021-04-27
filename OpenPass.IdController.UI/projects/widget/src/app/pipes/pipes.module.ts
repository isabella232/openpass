import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UseDeployUrlPipe } from './use-deploy-url.pipe';

@NgModule({
  declarations: [UseDeployUrlPipe],
  imports: [CommonModule],
  exports: [UseDeployUrlPipe],
})
export class PipesModule {}
