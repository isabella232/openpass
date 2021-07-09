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
  {
    path: 'reset',
    loadChildren: () => import('./containers/reset/reset.module').then((m) => m.ResetModule),
  },
  {
    path: 'under-construction',
    loadChildren: () =>
      import('./containers/under-construction/under-construction.module').then((m) => m.UnderConstructionModule),
  },
  {
    path: 'settings',
    loadChildren: () => import('./containers/settings/settings.module').then((m) => m.SettingsModule),
  },
  {
    path: 'redirect',
    loadChildren: () => import('./containers/redirect/redirect.module').then((m) => m.RedirectModule),
  },
  {
    path: '**',
    loadChildren: () => import('./containers/not-found/not-found.module').then((m) => m.NotFoundModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
