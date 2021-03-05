import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecognizedViewComponent } from './recognized-view.component';

const routes: Routes = [
  {
    path: '',
    component: RecognizedViewComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RecognizedViewRoutingModule {}
