import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { MainViewComponent } from './main-view.component';
import { MainViewRoutingModule } from './main-view-routing.module';

@NgModule({
  declarations: [MainViewComponent],
  imports: [CommonModule, MainViewRoutingModule, TranslateModule],
})
export class MainViewModule {}
