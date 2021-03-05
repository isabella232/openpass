import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnauthenticatedComponent } from './unauthenticated.component';
import { UnauthenticatedRoutingModule } from './unauthenticated-routing.module';

@NgModule({
  declarations: [UnauthenticatedComponent],
  imports: [CommonModule, UnauthenticatedRoutingModule],
})
export class UnauthenticatedModule {}
