import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '@components/shared/shared.module';
import { GoogleAuthModule } from '@components/google-auth/google-auth.module';
import { FacebookAuthModule } from '@components/facebook-auth/facebook-auth.module';
import { OpenPassDetailsModule } from '@components/open-pass-details/open-pass-details.module';
import { MainViewComponent } from './main-view.component';
import { MainViewRoutingModule } from './main-view-routing.module';

@NgModule({
  declarations: [MainViewComponent],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule,
    GoogleAuthModule,
    FacebookAuthModule,
    MainViewRoutingModule,
    OpenPassDetailsModule,
  ],
})
export class MainViewModule {}
