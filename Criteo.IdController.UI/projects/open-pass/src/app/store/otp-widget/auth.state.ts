import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import { AuthenticatedService } from '@rest/authenticated/authenticated.service';
import {
  GenerateCode,
  GenerateCodeFail,
  GenerateCodeSuccess,
  GetTokenByEmail,
  GetTokenByEmailFailed,
  SetCode,
  SetEmail,
  SetToken,
  ValidateCode,
  ValidateCodeFail,
  ReceiveToken,
} from './auth.actions';
import { localStorage } from '@shared/utils/storage-decorator';
import { EventTypes } from '@enums/event-types.enum';

export interface IAuthState {
  email: string;
  code: string;
  token: string;
  isFetching: boolean;
  isCodeValid: boolean;
  isEmailValid: boolean;
  isEmailVerified: boolean;
}

type LocalStateModel = IAuthState;
type LocalStateContext = StateContext<IAuthState>;

const defaults: IAuthState = {
  email: '',
  code: '',
  token: '',
  isFetching: false,
  isCodeValid: true,
  isEmailValid: true,
  isEmailVerified: false,
};

@State<IAuthState>({
  name: 'authState',
  defaults,
})
@Injectable()
export class AuthState {
  @localStorage('openpass.token')
  private storageUserToken: string;

  @localStorage('openpass.email')
  private storageUserEmail: string;

  constructor(private authenticatedService: AuthenticatedService) {}

  @Selector()
  static fullState(state: LocalStateModel): IAuthState {
    return state;
  }

  @Action(SetEmail)
  setEmail(ctx: LocalStateContext, { email }: SetEmail) {
    ctx.patchState({ email });
  }

  @Action(SetCode)
  setCode(ctx: LocalStateContext, { code }: SetCode) {
    ctx.patchState({ code });
  }

  @Action(SetToken)
  setToken(ctx: LocalStateContext, { token }: SetToken) {
    ctx.patchState({ token });
  }

  @Action(GenerateCode)
  generateCode(ctx: LocalStateContext) {
    ctx.patchState({ isFetching: true, isEmailValid: true });
    const { email } = ctx.getState();

    return this.authenticatedService.generateOtp({ email }).pipe(
      switchMap(() => ctx.dispatch(new GenerateCodeSuccess())),
      catchError((error) => ctx.dispatch(new GenerateCodeFail(error))),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(GenerateCodeSuccess)
  generateCodeSuccess(ctx: LocalStateContext) {
    ctx.patchState({ isEmailVerified: true });
  }

  @Action(GenerateCodeFail)
  generateCodeFail(ctx: LocalStateContext) {
    ctx.patchState({ isEmailValid: false });
  }

  @Action(ValidateCode)
  validateCode(ctx: LocalStateContext) {
    ctx.patchState({ isFetching: true, isCodeValid: true });
    const { email, code: otp } = ctx.getState();

    return this.authenticatedService.validateOtp({ email, otp }).pipe(
      switchMap(({ token }) => ctx.dispatch(new ReceiveToken(token))),
      catchError(() => ctx.dispatch(new ValidateCodeFail())),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(ReceiveToken)
  validateCodeSuccess(ctx: LocalStateContext, { token }: ReceiveToken) {
    const { email } = ctx.getState();
    this.storageUserToken = token;
    this.storageUserEmail = email;
    ctx.patchState({ token });
  }

  @Action(ValidateCodeFail)
  validateCodeFail(ctx: LocalStateContext) {
    ctx.patchState({ isCodeValid: false });
  }

  @Action(GetTokenByEmail)
  getTokenByEmail(ctx: LocalStateContext, { email, eventType }: GetTokenByEmail) {
    ctx.patchState({
      isFetching: true,
      email,
    });

    return this.authenticatedService.getTokenByEmail(email, eventType).pipe(
      switchMap(({ token }) => ctx.dispatch(new ReceiveToken(token))),
      catchError((error) => ctx.dispatch(new GetTokenByEmailFailed(error))),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }
}
