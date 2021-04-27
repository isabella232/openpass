import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UnauthenticatedComponent } from './unauthenticated.component';
import { RecognizedGuard } from '../../guards/recognized.guard';

const routes: Routes = [
  {
    path: '',
    component: UnauthenticatedComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadChildren: () => import('./main-view/main-view.module').then((m) => m.MainViewModule),
        canActivate: [RecognizedGuard],
      },
      {
        path: 'agreement',
        loadChildren: () => import('./agreement-view/agreement-view.module').then((m) => m.AgreementViewModule),
      },
      {
        path: 'recognized',
        loadChildren: () => import('./recognized-view/recognized-view.module').then((m) => m.RecognizedViewModule),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UnauthenticatedRoutingModule {}
