import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SuccessSignedViewRoutingModule } from './success-signed-view-routing.module';
import { SuccessSignedViewComponent } from './success-signed-view.component';

@NgModule({
  declarations: [SuccessSignedViewComponent],
  imports: [CommonModule, SuccessSignedViewRoutingModule],
})
export class SuccessSignedViewModule {}
