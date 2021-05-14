import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ResetComponent } from './reset.component';
import { GetScriptsResolver } from '../../resolvers/get-scripts.resolver';

const routes: Routes = [
  {
    path: '',
    component: ResetComponent,
    resolve: [GetScriptsResolver],
    data: {
      preloadScripts: ['https://apis.google.com/js/platform.js', 'https://connect.facebook.net/{{browserLang}}/sdk.js'],
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ResetRoutingModule {}
