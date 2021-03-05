import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '@components/shared/shared.module';
import { RecognizedViewComponent } from './recognized-view.component';
import { RecognizedViewRoutingModule } from './recognized-view-routing.module';

@NgModule({
  declarations: [RecognizedViewComponent],
  imports: [CommonModule, RecognizedViewRoutingModule, TranslateModule, SharedModule],
})
export class RecognizedViewModule {}
