const scope = '[IFA]';

export class GetIfa {
  static readonly type = `${scope} Get Ifa`;
}

export class GetIfaSuccess {
  static readonly type = `${scope} Get Ifa Success`;
  constructor(public readonly token: string) {}
}

export class GetIfaFailure {
  static readonly type = `${scope} Get Ifa Failure`;
}
