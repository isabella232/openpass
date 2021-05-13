import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { MainViewComponent } from './main-view.component';
import { MainViewRoutingModule } from './main-view-routing.module';
import { OpenPassDetailsModule } from '../../../components/open-pass-details/open-pass-details.module';
import { SharedModule } from '../../../components/shared/shared.module';
import { FacebookAuthSmallModule } from '../../../components/facebook-auth-small/facebook-auth-small.module';
import { GoogleAuthSmallModule } from '../../../components/google-auth-small/google-auth-small.module';

@NgModule({
  declarations: [MainViewComponent],
  imports: [
    CommonModule,
    MainViewRoutingModule,
    TranslateModule,
    OpenPassDetailsModule,
    SharedModule,
    FacebookAuthSmallModule,
    GoogleAuthSmallModule,
  ],
})
export class MainViewModule {}
