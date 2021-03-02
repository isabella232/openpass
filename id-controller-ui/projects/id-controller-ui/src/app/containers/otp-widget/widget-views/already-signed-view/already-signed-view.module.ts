import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AlreadySignedViewRoutingModule } from './already-signed-view-routing.module';
import { AlreadySignedViewComponent } from './already-signed-view.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@components/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [AlreadySignedViewComponent],
  imports: [CommonModule, AlreadySignedViewRoutingModule, FormsModule, SharedModule, TranslateModule],
})
export class AlreadySignedViewModule {}
