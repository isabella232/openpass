import { defaultEnv } from './default';

const appHost = 'https://my-advertising-experience.crto.in';

export const environment = {
  ...defaultEnv,
  production: true,
  webComponentHost: appHost + '/open-pass/widget',
  idControllerAppUrl: appHost + '/open-pass',
};
