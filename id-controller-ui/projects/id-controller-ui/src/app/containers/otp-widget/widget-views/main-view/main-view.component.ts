import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent {
  @Output() proceed = new EventEmitter<void>();

  websiteName = 'Website Name';
}
