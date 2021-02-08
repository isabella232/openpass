import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MainViewRoutingModule } from './main-view-routing.module';
import { MainViewComponent } from './main-view.component';

@NgModule({
  declarations: [MainViewComponent],
  imports: [CommonModule, MainViewRoutingModule],
})
export class MainViewModule {}
