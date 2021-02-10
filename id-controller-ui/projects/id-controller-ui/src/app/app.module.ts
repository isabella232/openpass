import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';
import { windowFactory } from '@utils/window-factory';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, RouterModule, AppRoutingModule],
  providers: [{ provide: 'Window', useFactory: windowFactory }],
  bootstrap: [AppComponent],
})
export class AppModule {}
