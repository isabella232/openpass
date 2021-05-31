import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '@components/shared/shared.module';
import { OpenPassDetailsModule } from '@components/open-pass-details/open-pass-details.module';
import { AgreementViewComponent } from './agreement-view.component';
import { AgreementViewRoutingModule } from './agreement-view-routing.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [AgreementViewComponent],
  imports: [
    CommonModule,
    AgreementViewRoutingModule,
    SharedModule,
    TranslateModule,
    OpenPassDetailsModule,
    FormsModule,
  ],
})
export class AgreementViewModule {}
