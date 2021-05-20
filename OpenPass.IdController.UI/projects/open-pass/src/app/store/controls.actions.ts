import { HttpErrorResponse } from '@angular/common/http';

const scope = '[Controls]';

export class PerformOptOut {
  static readonly type = `${scope} Perform OptOut`;
}

export class OptOutSuccess {
  static readonly type = `${scope} Successfully opted out`;
}

export class OptOutFailure {
  static readonly type = `${scope} OptOut failure`;

  constructor(public readonly error: HttpErrorResponse) {}
}
