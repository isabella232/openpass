import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthViewRoutingModule } from './auth-view-routing.module';
import { AuthViewComponent } from './auth-view.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@components/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [AuthViewComponent],
  imports: [CommonModule, AuthViewRoutingModule, FormsModule, SharedModule, TranslateModule],
})
export class AuthViewModule {}
