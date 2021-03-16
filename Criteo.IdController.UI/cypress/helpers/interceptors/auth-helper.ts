enum AuthMocks {
  generate = 'generate',
  validate = 'validate',
  getIfa = 'getIfa',
}

export class AuthHelper {
  static mockGenerateCode(response: { [p: string]: any } = {}): string {
    cy.intercept('POST', '**/otp/generate', response).as(AuthMocks.generate);

    return '@' + AuthMocks.generate;
  }

  static mockValidateCode(response: { [p: string]: any } = {}) {
    cy.intercept('POST', '**/otp/validate', response).as(AuthMocks.validate);

    return '@' + AuthMocks.validate;
  }

  static mockGetIfa(response: { [p: string]: any } = {}) {
    // specify /api to avoid conflicts with front-end routes
    cy.intercept('GET', '**/api/unauthenticated', response).as(AuthMocks.getIfa);

    return '@' + AuthMocks.getIfa;
  }
}
