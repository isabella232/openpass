import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import { OtpService } from '@rest/otp/otp.service';
import {
  GenerateCode,
  GenerateCodeFail,
  GenerateCodeSuccess,
  SetCode,
  SetEmail,
  SetShareEmailValue,
  SetToken,
  ValidateCode,
  ValidateCodeFail,
  ValidateCodeSuccess,
} from './auth.actions';
import { localStorage } from '@shared/utils/storage-decorator';

export interface IAuthState {
  email: string;
  code: string;
  token: string;
  shareEmail: boolean;
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
  shareEmail: true,
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

  @localStorage('openpass.share-email')
  private storageUserShareEmail: boolean;

  constructor(private otpService: OtpService) {}

  @Selector()
  static fullState(state: LocalStateModel): IAuthState {
    return state;
  }

  @Action(SetEmail)
  setEmail(ctx: LocalStateContext, { email }: SetEmail) {
    ctx.patchState({ email });
  }

  @Action(SetShareEmailValue)
  setShareEmail(ctx: LocalStateContext, { shareEmail }: SetShareEmailValue) {
    ctx.patchState({ shareEmail });
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

    return this.otpService.generateOtp({ email }).pipe(
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

    return this.otpService.validateOtp({ email, otp }).pipe(
      switchMap(({ token }) => ctx.dispatch(new ValidateCodeSuccess(token))),
      catchError(() => ctx.dispatch(new ValidateCodeFail())),
      finalize(() => ctx.patchState({ isFetching: false }))
    );
  }

  @Action(ValidateCodeSuccess)
  validateCodeSuccess(ctx: LocalStateContext, { token }: ValidateCodeSuccess) {
    const { email, shareEmail } = ctx.getState();
    this.storageUserToken = token;
    this.storageUserEmail = email;
    this.storageUserShareEmail = shareEmail;
    ctx.patchState({ token });
  }

  @Action(ValidateCodeFail)
  validateCodeFail(ctx: LocalStateContext) {
    ctx.patchState({ isCodeValid: false });
  }
}
