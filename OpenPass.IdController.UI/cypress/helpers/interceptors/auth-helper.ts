enum AuthMocks {
  generate = 'generate',
  validate = 'validate',
  getIfa = 'getIfa',
}

// Specify '/api' in all stubs to avoid conflicts with front-end routes
export class AuthHelper {
  static mockGenerateCode(response: { [p: string]: any } = {}): string {
    cy.intercept('POST', '**/api/authenticated/otp/generate', response).as(AuthMocks.generate);

    return '@' + AuthMocks.generate;
  }

  static mockValidateCode(response: { [p: string]: any } = {}) {
    cy.intercept('POST', '**/api/authenticated/otp/validate', response).as(AuthMocks.validate);

    return '@' + AuthMocks.validate;
  }

  static mockGetIfa(response: { [p: string]: any } = {}) {
    cy.intercept('GET', '**/api/unauthenticated', response).as(AuthMocks.getIfa);

    return '@' + AuthMocks.getIfa;
  }
}
