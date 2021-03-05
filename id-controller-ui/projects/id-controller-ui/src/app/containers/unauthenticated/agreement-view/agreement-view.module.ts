import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AgreementViewRoutingModule } from './agreement-view-routing.module';
import { AgreementViewComponent } from './agreement-view.component';
import { SharedModule } from '../../../components/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { OpenPassDetailsModule } from '../../../components/open-pass-details/open-pass-details.module';

@NgModule({
  declarations: [AgreementViewComponent],
  imports: [CommonModule, AgreementViewRoutingModule, SharedModule, TranslateModule, OpenPassDetailsModule],
})
export class AgreementViewModule {}
