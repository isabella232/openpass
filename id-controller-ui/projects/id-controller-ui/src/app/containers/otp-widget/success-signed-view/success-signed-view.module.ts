import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { SuccessSignedViewComponent } from './success-signed-view.component';
import { SuccessSignedViewRoutingModule } from './success-signed-view-routing.module';

@NgModule({
  declarations: [SuccessSignedViewComponent],
  imports: [CommonModule, SuccessSignedViewRoutingModule, TranslateModule],
})
export class SuccessSignedViewModule {}
