import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./containers/otp-widget/otp-widget.module').then((m) => m.OtpWidgetModule),
  },
  {
    path: 'unauthenticated',
    loadChildren: () =>
      import('./containers/unauthenticated/unauthenticated.module').then((m) => m.UnauthenticatedModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
