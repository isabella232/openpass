import { Component, CUSTOM_ELEMENTS_SCHEMA, Input, NgModule, OnInit } from '@angular/core';
import { localStorage } from '@shared/utils/storage-decorator';
import { WidgetModes } from '@enums/widget-modes.enum';
import { Variants } from '@enums/variants.enum';
import { Sessions } from '@enums/sessions.enum';
import { Providers } from '@enums/providers.enum';
import { CommonModule } from '@angular/common';
import { ViewContainerModule } from '@directives/view-container.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'wdgt-dev-components',
  templateUrl: './dev-components.component.html',
  styleUrls: ['./dev-components.component.scss'],
})
export class DevComponentsComponent implements OnInit {
  @Input()
  items: string[];

  @localStorage<string>('dev.active-component')
  selectedComponent: string;
  @localStorage<string>('dev.ng-content')
  projectedNode = '<h1>Salut!</h1><p>This is a snack bar message</p>';
  @localStorage<string>('dev.brandImageUrl')
  brandImageUrl: string;
  @localStorage<string>('dev.brandColor')
  brandColor: string;
  @localStorage<Providers>('dev.provider')
  provider: Providers;
  @localStorage<Sessions>('dev.session')
  session: Sessions;
  @localStorage<Variants>('dev.variant')
  variant: Variants;
  @localStorage<WidgetModes>('dev.view')
  view: WidgetModes;

  providerOptions = Object.values(Providers);
  sessionOptions = Object.values(Sessions);
  variantOptions = Object.values(Variants);
  viewOptions = Object.values(WidgetModes);

  ngOnInit(): void {
    this.selectedComponent ??= this.items[0];
  }
}

@NgModule({
  declarations: [DevComponentsComponent],
  exports: [DevComponentsComponent],
  imports: [CommonModule, ViewContainerModule, FormsModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
class DevComponentsModule {}
