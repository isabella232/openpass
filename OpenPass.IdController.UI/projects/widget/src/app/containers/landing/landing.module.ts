import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LandingComponent } from './landing.component';
import { PipesModule } from '../../pipes/pipes.module';
import { SliderModule } from '../../components/slider/slider.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  imports: [CommonModule, PipesModule, SliderModule, TranslateModule],
  declarations: [LandingComponent],
  exports: [LandingComponent],
})
export class LandingModule {}
