import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { ReadUserData, SetFetchGoogleApi } from '@store/otp-widget/sso.actions';

export interface ISsoState {
  isSignedIn: boolean;
  userEmail: string;
  isFetching: boolean;
}

type LocalStateModel = ISsoState;
type LocalStateContext = StateContext<ISsoState>;

const defaults: ISsoState = {
  isFetching: false,
  userEmail: '',
  isSignedIn: false,
};

@State<ISsoState>({
  name: 'ssoState',
  defaults,
})
@Injectable()
export class SsoState {
  @Selector()
  static isSignedIn({ isSignedIn }: LocalStateModel) {
    return isSignedIn;
  }

  @Selector()
  static userEmail({ userEmail }: LocalStateModel) {
    return userEmail;
  }

  @Selector()
  static isFetching({ isFetching }: LocalStateModel) {
    return isFetching;
  }

  @Action(ReadUserData)
  readUserData(ctx: LocalStateContext, userData: ReadUserData) {
    ctx.patchState({
      isFetching: false,
      userEmail: userData.email,
      isSignedIn: userData.isSignedIn,
    });
  }

  @Action(SetFetchGoogleApi)
  setFetchGoogleApi(ctx: LocalStateContext, { isFetching }: SetFetchGoogleApi) {
    ctx.patchState({ isFetching });
  }
}
