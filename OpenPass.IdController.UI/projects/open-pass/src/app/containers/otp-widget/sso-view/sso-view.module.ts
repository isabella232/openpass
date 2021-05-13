import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SsoViewComponent } from './sso-view.component';
import { SsoViewRoutingModule } from './sso-view-routing.module';
import { GoogleAuthButtonModule } from '@components/google-auth-button/google-auth-button.module';
import { FacebookAuthButtonModule } from '@components/facebook-auth-button/facebook-auth-button.module';
import { SharedModule } from '@components/shared/shared.module';

@NgModule({
  declarations: [SsoViewComponent],
  imports: [
    CommonModule,
    SsoViewRoutingModule,
    GoogleAuthButtonModule,
    FacebookAuthButtonModule,
    TranslateModule,
    SharedModule,
  ],
})
export class SsoViewModule {}
