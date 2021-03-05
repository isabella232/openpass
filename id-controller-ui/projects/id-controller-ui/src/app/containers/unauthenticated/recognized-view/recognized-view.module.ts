import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RecognizedViewRoutingModule } from './recognized-view-routing.module';
import { RecognizedViewComponent } from './recognized-view.component';

@NgModule({
  declarations: [RecognizedViewComponent],
  imports: [CommonModule, RecognizedViewRoutingModule],
})
export class RecognizedViewModule {}
