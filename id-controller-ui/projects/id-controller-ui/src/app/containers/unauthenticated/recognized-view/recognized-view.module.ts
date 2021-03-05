import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { RecognizedViewRoutingModule } from './recognized-view-routing.module';
import { RecognizedViewComponent } from './recognized-view.component';
import { SharedModule } from '../../../components/shared/shared.module';

@NgModule({
  declarations: [RecognizedViewComponent],
  imports: [CommonModule, RecognizedViewRoutingModule, TranslateModule, SharedModule],
})
export class RecognizedViewModule {}
