const scope = '[Opener]';
type Config = { view: string; origin: string; variant: string; session: string; provider: string };

export class SaveOpener {
  static readonly type = `${scope} Save origin`;

  constructor(public readonly origin: string) {}
}

export class SetConfig {
  static readonly type = `${scope} Save config`;

  constructor(public readonly config: Config) {}
}
