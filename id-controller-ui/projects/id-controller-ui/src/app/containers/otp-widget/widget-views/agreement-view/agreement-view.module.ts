import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AgreementViewRoutingModule } from './agreement-view-routing.module';
import { AgreementViewComponent } from './agreement-view.component';

@NgModule({
  declarations: [AgreementViewComponent],
  imports: [CommonModule, AgreementViewRoutingModule],
})
export class AgreementViewModule {}
