import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SsoViewComponent } from './sso-view.component';
import { SsoViewRoutingModule } from './sso-view-routing.module';

@NgModule({
  declarations: [SsoViewComponent],
  imports: [CommonModule, SsoViewRoutingModule],
})
export class SsoViewModule {}
