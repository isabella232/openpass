const scope = '[Opener]';

export class SaveOpener {
  static readonly type = `${scope} Save origin`;

  constructor(public readonly origin: string) {}
}
