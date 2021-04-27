import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SsoViewComponent } from './sso-view.component';

const routes: Routes = [
  {
    path: '',
    component: SsoViewComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SsoViewRoutingModule {}
