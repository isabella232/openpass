import { Component, NgModule, OnInit } from '@angular/core';
import { DynamicLoadable } from '../dynamic-loadable';
import { WidgetModes } from '../../enums/widget-modes.enum';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'wdgt-unlogged',
  templateUrl: './unlogged.component.html',
  styleUrls: ['./unlogged.component.scss'],
})
export class UnloggedComponent implements OnInit, DynamicLoadable {
  view: WidgetModes;

  constructor() {}

  ngOnInit() {}
}

@NgModule({
  declarations: [UnloggedComponent],
  imports: [CommonModule],
})
class UnloggedModule {}
