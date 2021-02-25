import { Component, EventEmitter, Output } from '@angular/core';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';
import { OpenerState } from '@store/otp-widget/opener.state';

@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent {
  @Output() proceed = new EventEmitter<void>();

  @Select(OpenerState.originFormatted) websiteName$: Observable<string>;
}
