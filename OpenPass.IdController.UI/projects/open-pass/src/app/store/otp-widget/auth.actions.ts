import { HttpErrorResponse } from '@angular/common/http';
import { EventTypes } from '@enums/event-types.enum';

const scope = '[Auth]';

export class SetEmail {
  static readonly type = `${scope} Set Email`;

  constructor(public readonly email: string) {}
}

export class SetCode {
  static readonly type = `${scope} Set code`;

  constructor(public readonly code: string) {}
}

export class SetToken {
  static readonly type = `${scope} Set token`;

  constructor(public readonly token: string) {}
}

export class GenerateCode {
  static readonly type = `${scope} Generate code`;
}

export class GenerateCodeSuccess {
  static readonly type = `${scope} Generate code. Success`;
}

export class GenerateCodeFail {
  static readonly type = `${scope} Generate code. Fail`;

  constructor(public readonly error: HttpErrorResponse) {}
}

export class ValidateCode {
  static readonly type = `${scope} Validate code`;
}

export class ReceiveToken {
  static readonly type = `${scope} Token received`;

  constructor(public readonly token: string) {}
}

export class ValidateCodeFail {
  static readonly type = `${scope} Validate code. Fail`;
}

export class GetTokenByEmail {
  static readonly type = `${scope} Get Token By Email`;

  constructor(public readonly email: string, public readonly eventType: EventTypes) {}
}

export class GetTokenByEmailFailed {
  static readonly type = `${scope} Get Token By Email. Fail`;

  constructor(public readonly error: HttpErrorResponse) {}
}
