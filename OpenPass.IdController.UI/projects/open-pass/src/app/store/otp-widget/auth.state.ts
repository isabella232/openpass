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
  SetAuthDefault,
  GetAnonymousTokens,
  GetAnonymousTokensSuccess,
  GetAnonymousTokensFailure,
} from './auth.actions';
import { localStorage } from '@shared/utils/storage-decorator';
import { UnauthenticatedService } from '@rest/unauthenticated/unauthenticated.service';

export interface IAuthState {
  email: string;
  code: string;
  ifaToken: string;
  uid2Token: string;
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
  ifaToken: '',
  uid2Token: '',
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
  @localStorage('openpass.uid2Token')
  private storageUid2Token: string;
  @localStorage('openpass.ifaToken')
  private storageIfaToken: string;

  @localStorage('openpass.email')
  private storageUserEmail: string;

  constructor(
    private authenticatedService: AuthenticatedService,
    private unauthenticatedService: UnauthenticatedService
  ) {}

  @Selector()
  static fullState(state: LocalStateModel): IAuthState {
    return state;
  }

  @Selector()
  static isFetching({ isFetching }: LocalStateModel) {
    return isFetching;
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
  setToken(ctx: LocalStateContext, { tokens: { ifaToken, uid2Token } }: SetToken) {
    ctx.patchState({ ifaToken, uid2Token });
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
      switchMap((tokens) => ctx.dispatch(new ReceiveToken(tokens))),
      catchError(() => ctx.dispatch(new ValidateCodeFail())),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(ReceiveToken)
  validateCodeSuccess(ctx: LocalStateContext, { tokens: { ifaToken, uid2Token } }: ReceiveToken) {
    const { email } = ctx.getState();
    this.storageIfaToken = ifaToken;
    this.storageUid2Token = uid2Token;
    this.storageUserEmail = email;
    ctx.patchState({ ifaToken, uid2Token });
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
      switchMap((tokens) => ctx.dispatch(new ReceiveToken(tokens))),
      catchError((error) => ctx.dispatch(new GetTokenByEmailFailed(error))),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(GetAnonymousTokens)
  getAnonymousTokens(ctx: LocalStateContext) {
    ctx.patchState({ isFetching: true });
    return this.unauthenticatedService.createIfa().pipe(
      switchMap((tokens) => ctx.dispatch(new GetAnonymousTokensSuccess(tokens))),
      catchError((error) => ctx.dispatch(new GetAnonymousTokensFailure(error))),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(GetAnonymousTokensSuccess)
  getAnonymousTokensSuccess(ctx: LocalStateContext, { tokens: { ifaToken, uid2Token } }: GetAnonymousTokensSuccess) {
    this.storageIfaToken = ifaToken;
    this.storageUid2Token = uid2Token;
    ctx.patchState({ ifaToken, uid2Token });
  }

  @Action(SetAuthDefault)
  setAuthDefault(ctx: LocalStateContext) {
    ctx.patchState({
      email: '',
      code: '',
      isFetching: false,
      isCodeValid: true,
      isEmailValid: true,
      isEmailVerified: false,
    });
  }
}
