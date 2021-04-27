import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AgreementViewComponent } from './agreement-view.component';

const routes: Routes = [
  {
    path: '',
    component: AgreementViewComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AgreementViewRoutingModule {}
