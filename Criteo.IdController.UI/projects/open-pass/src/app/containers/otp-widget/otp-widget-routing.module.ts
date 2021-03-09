import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OtpWidgetComponent } from './otp-widget.component';
import { AuthenticatedGuard } from '../../guards/authenticated.guard';
import { GuestGuard } from '../../guards/guest.guard';

const routes: Routes = [
  {
    path: '',
    component: OtpWidgetComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadChildren: () => import('./main-view/main-view.module').then((m) => m.MainViewModule),
      },
      {
        path: 'auth',
        loadChildren: () => import('./auth-view/auth-view.module').then((m) => m.AuthViewModule),
        canActivate: [GuestGuard],
      },
      {
        path: 'agreement',
        loadChildren: () => import('./agreement-view/agreement-view.module').then((m) => m.AgreementViewModule),
      },
      {
        path: 'success',
        loadChildren: () =>
          import('./success-signed-view/success-signed-view.module').then((m) => m.SuccessSignedViewModule),
      },
      {
        path: 'signed',
        loadChildren: () =>
          import('./already-signed-view/already-signed-view.module').then((m) => m.AlreadySignedViewModule),
        canActivate: [AuthenticatedGuard],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OtpWidgetRoutingModule {}