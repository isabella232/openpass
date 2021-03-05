import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UnauthenticatedComponent } from './unauthenticated.component';

const routes: Routes = [
  {
    path: '',
    component: UnauthenticatedComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadChildren: () => import('./main-view/main-view.module').then((m) => m.MainViewModule),
      },
      {
        path: 'agreement',
        loadChildren: () => import('./agreement-view/agreement-view.module').then((m) => m.AgreementViewModule),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UnauthenticatedRoutingModule {}
