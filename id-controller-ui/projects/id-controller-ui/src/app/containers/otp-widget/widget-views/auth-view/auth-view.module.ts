import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthViewRoutingModule } from './auth-view-routing.module';
import { AuthViewComponent } from './auth-view.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@components/shared/shared.module';

@NgModule({
  declarations: [AuthViewComponent],
  imports: [CommonModule, AuthViewRoutingModule, FormsModule, SharedModule],
})
export class AuthViewModule {}
