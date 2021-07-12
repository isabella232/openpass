import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SnackBarComponent } from './snack-bar.component';
import { PipesModule } from '@pipes/pipes.module';

@NgModule({
  declarations: [SnackBarComponent],
  imports: [CommonModule, PipesModule],
})
export class SnackBarModule {}
