import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { RedirectComponent } from './redirect.component';
import { RedirectRoutingModule } from './redirect-routing.module';

@NgModule({
  declarations: [RedirectComponent],
  imports: [CommonModule, RouterModule, TranslateModule, RedirectRoutingModule],
})
export class RedirectModule {}
