const scope = '[Google SSO]';

export class ReadUserData {
  static readonly type = `${scope} Read User data`;

  constructor(public readonly email: string, public readonly isSignedIn: boolean) {}
}

export class SetFetchGoogleApi {
  static readonly type = `${scope} Set isFetching flag`;

  constructor(public readonly isFetching: boolean) {}
}
