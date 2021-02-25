import { Component, OnInit } from '@angular/core';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';
import { OpenerState } from '@store/otp-widget/opener.state';

@Component({
  selector: 'usrf-agreement-view',
  templateUrl: './agreement-view.component.html',
  styleUrls: ['./agreement-view.component.scss'],
})
export class AgreementViewComponent implements OnInit {
  @Select(OpenerState.originFormatted) websiteName$: Observable<string>;
  constructor() {}

  ngOnInit(): void {}
}
