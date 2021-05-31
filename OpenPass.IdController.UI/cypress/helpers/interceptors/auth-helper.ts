enum AuthMocks {
  generate = 'generate',
  validate = 'validate',
  createIfa = 'createIfa',
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

  static mockCreateIfa(response: { [p: string]: any } = {}) {
    cy.intercept('POST', '**/api/unauthenticated', response).as(AuthMocks.createIfa);

    return '@' + AuthMocks.createIfa;
  }
}
