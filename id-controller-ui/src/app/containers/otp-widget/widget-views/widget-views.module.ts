import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainViewComponent } from './main-view/main-view.component';
import { AuthViewComponent } from './auth-view/auth-view.component';
import { AgreementViewComponent } from './agreement-view/agreement-view.component';
import { SuccessSignedViewComponent } from './success-signed-view/success-signed-view.component';
import { AlreadySignedViewComponent } from './already-signed-view/already-signed-view.component';

@NgModule({
  declarations: [
    MainViewComponent,
    AuthViewComponent,
    AgreementViewComponent,
    SuccessSignedViewComponent,
    AlreadySignedViewComponent,
  ],
  exports: [
    MainViewComponent,
    AuthViewComponent,
    AgreementViewComponent,
    SuccessSignedViewComponent,
    AlreadySignedViewComponent,
  ],
  imports: [CommonModule],
})
export class WidgetViewsModule {}
