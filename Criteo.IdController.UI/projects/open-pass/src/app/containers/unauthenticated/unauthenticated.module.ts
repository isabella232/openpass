import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnauthenticatedComponent } from './unauthenticated.component';
import { UnauthenticatedRoutingModule } from './unauthenticated-routing.module';
import { NavigationModule } from '../../components/navigation/navigation.module';

@NgModule({
  declarations: [UnauthenticatedComponent],
  imports: [CommonModule, UnauthenticatedRoutingModule, NavigationModule],
})
export class UnauthenticatedModule {}
