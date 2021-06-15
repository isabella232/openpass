import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { SaveOpener, SetConfig } from './opener.actions';
import { AuthState, IAuthState } from '@store/otp-widget/auth.state';

interface IStateModel {
  ifa: string;
  uid2: string;
  view: string;
  origin: string;
  variant: string;
  session: string;
  provider: string;
  ctoBundle: string;
}

type StateModel = IStateModel;
type LocalStateContext = StateContext<IStateModel>;

const defaults: StateModel = {
  ifa: undefined,
  uid2: undefined,
  view: undefined,
  origin: undefined,
  variant: undefined,
  session: undefined,
  provider: undefined,
  ctoBundle: undefined,
};

@State<IStateModel>({
  name: 'originState',
  defaults,
})
@Injectable()
export class OpenerState {
  @Selector()
  static fullState(state: StateModel): StateModel {
    return state;
  }

  @Selector([AuthState.fullState])
  static config(state: StateModel, authState: IAuthState): Partial<StateModel> {
    const { view, variant, session, provider, ctoBundle } = state;
    const ifa = state.ifa || authState.ifaToken;
    const uid2 = state.uid2 || authState.uid2Token;
    return { view, variant, session, provider, ifa, uid2, ctoBundle };
  }

  @Selector()
  static origin({ origin }: StateModel): string {
    return origin;
  }

  @Selector()
  static originFormatted({ origin }: StateModel): string {
    return new URL(origin).hostname;
  }

  @Action(SaveOpener)
  saveOpener(ctx: LocalStateContext, { origin }: SaveOpener) {
    ctx.patchState({ origin });
  }

  @Action(SetConfig)
  setConfig(ctx: LocalStateContext, { config }: SetConfig) {
    ctx.patchState(config);
  }
}
