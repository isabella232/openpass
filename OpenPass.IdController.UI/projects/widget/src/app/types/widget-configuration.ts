import { Variants } from '../enums/variants.enum';
import { Sessions } from '../enums/sessions.enum';
import { Providers } from '../enums/providers.enum';
import { WidgetModes } from '../enums/widget-modes.enum';

export type WidgetConfiguration = {
  view: WidgetModes;
  variant: Variants;
  session: Sessions;
  provider: Providers;
};
