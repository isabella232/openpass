import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { GetIfa, GetIfaFailure, GetIfaSuccess } from './ifa.actions';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import { localStorage } from '@shared/utils/storage-decorator';
import { UnauthenticatedService } from '@rest/unauthenticated/unauthenticated.service';

export interface IIfaState {
  token: string;
  isFetching: boolean;
}

type LocalModel = IIfaState;
type LocalStateContext = StateContext<IIfaState>;

const defaults: LocalModel = {
  token: undefined,
  isFetching: false,
};

@State<IIfaState>({
  name: 'ifaState',
  defaults,
})
@Injectable()
export class IfaState {
  @localStorage('openpass.token')
  private storageUserToken: string;

  constructor(private unauthenticatedService: UnauthenticatedService) {}

  @Selector()
  static isFetching({ isFetching }: LocalModel) {
    return isFetching;
  }

  @Selector()
  static token({ token }: LocalModel) {
    return token;
  }

  @Action(GetIfa)
  getIfa(ctx: LocalStateContext) {
    ctx.patchState({ isFetching: true });
    return this.unauthenticatedService.createIfa().pipe(
      switchMap((ifaDto) => ctx.dispatch(new GetIfaSuccess(ifaDto.token))),
      catchError(() => ctx.dispatch(new GetIfaFailure())),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(GetIfaSuccess)
  getIfaSuccess(ctx: LocalStateContext, { token }: GetIfaSuccess) {
    this.storageUserToken = token;
    ctx.patchState({ token });
  }
}
