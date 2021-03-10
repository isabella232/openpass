import { Component, OnDestroy, OnInit } from '@angular/core';
import { Actions, ofActionDispatched, Select } from '@ngxs/store';
import { OpenerState } from '@store/otp-widget/opener.state';
import { Observable, Subject } from 'rxjs';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { GetIfa, GetIfaSuccess } from '@store/ifa/ifa.actions';
import { IfaState } from '@store/ifa/ifa.state';

@Component({
  selector: 'usrf-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
})
export class MainViewComponent implements OnInit, OnDestroy {
  @Select(OpenerState.originFormatted)
  websiteName$: Observable<string>;
  @Select(IfaState.isFetching)
  isFetching$: Observable<boolean>;

  isDestroyed = new Subject();

  constructor(private router: Router, private actions$: Actions) {}

  @Dispatch()
  fetchIfaAndProceed() {
    return new GetIfa();
  }

  ngOnInit() {
    this.actions$
      .pipe(ofActionDispatched(GetIfaSuccess), takeUntil(this.isDestroyed))
      .subscribe(() => this.router.navigate(['unauthenticated', 'agreement']));
  }

  ngOnDestroy() {
    this.isDestroyed.next();
  }
}
