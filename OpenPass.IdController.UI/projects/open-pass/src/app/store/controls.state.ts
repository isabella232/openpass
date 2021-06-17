import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { ControlsService } from '@rest/portal/controls.service';
import { OptOutFailure, OptOutSuccess, PerformOptOut } from './controls.actions';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import { AuthService } from '@services/auth.service';

export interface IControlsModel {
  isFetching: boolean;
}

type LocalModelState = IControlsModel;
type LocalStateContext = StateContext<IControlsModel>;

const defaults: LocalModelState = {
  isFetching: false,
};

@State<LocalModelState>({
  name: 'controlsState',
  defaults,
})
@Injectable()
export class ControlsState {
  constructor(private authService: AuthService, private controlsService: ControlsService) {}

  @Selector()
  static isFetching({ isFetching }: LocalModelState): boolean {
    return isFetching;
  }

  @Action(PerformOptOut)
  performOptOut(ctx: LocalStateContext) {
    ctx.patchState({ isFetching: true });

    return this.controlsService.optOut().pipe(
      switchMap(() => ctx.dispatch(new OptOutSuccess())),
      catchError((error) => ctx.dispatch(new OptOutFailure(error))),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(OptOutSuccess)
  optOutSuccess() {
    this.authService.resetToken();
  }
}
