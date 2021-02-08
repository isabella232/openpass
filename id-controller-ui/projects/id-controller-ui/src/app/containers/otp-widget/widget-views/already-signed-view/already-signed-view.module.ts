import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AlreadySignedViewRoutingModule } from './already-signed-view-routing.module';
import { AlreadySignedViewComponent } from './already-signed-view.component';

@NgModule({
  declarations: [AlreadySignedViewComponent],
  imports: [CommonModule, AlreadySignedViewRoutingModule],
})
export class AlreadySignedViewModule {}
