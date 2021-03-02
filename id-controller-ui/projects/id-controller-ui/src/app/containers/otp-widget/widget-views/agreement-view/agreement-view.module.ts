import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AgreementViewRoutingModule } from './agreement-view-routing.module';
import { AgreementViewComponent } from './agreement-view.component';
import { SharedModule } from '@components/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [AgreementViewComponent],
  imports: [CommonModule, AgreementViewRoutingModule, SharedModule, TranslateModule],
})
export class AgreementViewModule {}
