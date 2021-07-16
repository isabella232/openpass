import { NgModule } from '@angular/core';
import { ViewContainerDirective } from './view-container.directive';

@NgModule({
  declarations: [ViewContainerDirective],
  exports: [ViewContainerDirective],
})
export class ViewContainerModule {}
