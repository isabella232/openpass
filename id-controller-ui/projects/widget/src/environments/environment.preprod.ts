import { defaultEnv } from './default';

const appHost = 'https://my-advertising-experience.preprod.crto.in';

export const environment = {
  ...defaultEnv,
  production: true,
  webComponentHost: appHost + '/open-pass/preprod/widget',
  idControllerAppUrl: appHost + '/open-pass',
};
