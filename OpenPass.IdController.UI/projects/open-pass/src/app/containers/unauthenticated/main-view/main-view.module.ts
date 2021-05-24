import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { MainViewComponent } from './main-view.component';
import { MainViewRoutingModule } from './main-view-routing.module';
import { OpenPassDetailsModule } from '../../../components/open-pass-details/open-pass-details.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [MainViewComponent],
  imports: [CommonModule, MainViewRoutingModule, TranslateModule, OpenPassDetailsModule, FormsModule],
})
export class MainViewModule {}
