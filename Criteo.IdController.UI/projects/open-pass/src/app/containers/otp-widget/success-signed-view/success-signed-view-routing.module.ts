import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SuccessSignedViewComponent } from './success-signed-view.component';

const routes: Routes = [
  {
    path: '',
    component: SuccessSignedViewComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SuccessSignedViewRoutingModule {}
