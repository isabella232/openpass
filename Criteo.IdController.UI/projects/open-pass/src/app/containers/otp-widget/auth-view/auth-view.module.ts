import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '@components/shared/shared.module';
import { AuthViewComponent } from './auth-view.component';
import { AuthViewRoutingModule } from './auth-view-routing.module';

@NgModule({
  declarations: [AuthViewComponent],
  imports: [CommonModule, AuthViewRoutingModule, FormsModule, SharedModule, TranslateModule, RouterModule],
})
export class AuthViewModule {}
