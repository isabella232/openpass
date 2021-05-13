import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CopyrightComponent } from './copyright/copyright.component';
import { TranslateModule } from '@ngx-translate/core';
import { OrHrComponent } from './or-hr/or-hr.component';

@NgModule({
  declarations: [CopyrightComponent, OrHrComponent],
  exports: [CopyrightComponent, OrHrComponent],
  imports: [CommonModule, TranslateModule],
})
export class SharedModule {}
