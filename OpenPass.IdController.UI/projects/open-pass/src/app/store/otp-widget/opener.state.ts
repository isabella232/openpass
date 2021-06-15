import { Action, Selector, State, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { SaveOpener } from './opener.actions';

interface IStateModel {
  origin: string;
}

type StateModel = IStateModel;
type LocalStateContext = StateContext<IStateModel>;

const defaults: StateModel = {
  origin: undefined,
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
}
