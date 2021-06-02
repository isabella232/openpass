import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SsoViewComponent } from './sso-view.component';
import { SsoViewRoutingModule } from './sso-view-routing.module';
import { SharedModule } from '@components/shared/shared.module';
import { GoogleAuthModule } from '@components/google-auth/google-auth.module';
import { FacebookAuthModule } from '@components/facebook-auth/facebook-auth.module';

@NgModule({
  declarations: [SsoViewComponent],
  imports: [CommonModule, SharedModule, TranslateModule, GoogleAuthModule, FacebookAuthModule, SsoViewRoutingModule],
})
export class SsoViewModule {}
