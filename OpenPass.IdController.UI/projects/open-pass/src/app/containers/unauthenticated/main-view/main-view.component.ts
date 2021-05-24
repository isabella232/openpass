import { Component, OnDestroy, OnInit } from '@angular/core';
import { Actions, ofActionDispatched, Select, Store } from '@ngxs/store';
import { OpenerState } from '@store/otp-widget/opener.state';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { GetIfa, GetIfaSuccess } from '@store/ifa/ifa.actions';
import { IfaState } from '@store/ifa/ifa.state';
import { AuthService } from '@services/auth.service';
import { DialogWindowService } from '@services/dialog-window.service';

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
  acceptTerms = false;

  constructor(
    private store: Store,
    private actions$: Actions,
    private authService: AuthService,
    private dialogWindowService: DialogWindowService
  ) {}

  @Dispatch()
  fetchIfaAndProceed() {
    return new GetIfa();
  }

  ngOnInit() {
    this.actions$.pipe(ofActionDispatched(GetIfaSuccess), takeUntil(this.isDestroyed)).subscribe(() => this.confirm());
  }

  ngOnDestroy() {
    this.isDestroyed.next();
  }

  closeWindow() {
    this.dialogWindowService.closeDialogWindow(true);
  }

  private confirm() {
    this.authService.setTokenToOpener();
    this.dialogWindowService.closeDialogWindow();
  }
}
