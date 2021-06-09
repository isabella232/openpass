import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { SaveOpener, SetConfig } from './opener.actions';

interface IStateModel {
  view: string;
  origin: string;
  variant: string;
  session: string;
  provider: string;
}

type StateModel = IStateModel;
type LocalStateContext = StateContext<IStateModel>;

const defaults: StateModel = {
  view: undefined,
  origin: undefined,
  variant: undefined,
  session: undefined,
  provider: undefined,
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

  @Selector()
  static config({ view, variant, session, provider }: StateModel): Partial<StateModel> {
    return { view, variant, session, provider };
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
