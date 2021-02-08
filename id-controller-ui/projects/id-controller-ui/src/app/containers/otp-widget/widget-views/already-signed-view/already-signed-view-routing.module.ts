import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlreadySignedViewComponent } from './already-signed-view.component';

const routes: Routes = [
  {
    path: '',
    component: AlreadySignedViewComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AlreadySignedViewRoutingModule {}
