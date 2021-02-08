import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OtpWidgetComponent } from './otp-widget.component';

const routes: Routes = [
  {
    path: '',
    component: OtpWidgetComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadChildren: () => import('./widget-views/main-view/main-view.module').then((m) => m.MainViewModule),
      },
      {
        path: 'auth',
        loadChildren: () => import('./widget-views/auth-view/auth-view.module').then((m) => m.AuthViewModule),
      },
      {
        path: 'agreement',
        loadChildren: () =>
          import('./widget-views/agreement-view/agreement-view.module').then((m) => m.AgreementViewModule),
      },
      {
        path: 'success',
        loadChildren: () =>
          import('./widget-views/success-signed-view/success-signed-view.module').then(
            (m) => m.SuccessSignedViewModule
          ),
      },
      {
        path: 'signed',
        loadChildren: () =>
          import('./widget-views/already-signed-view/already-signed-view.module').then(
            (m) => m.AlreadySignedViewModule
          ),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OtpWidgetRoutingModule {}
