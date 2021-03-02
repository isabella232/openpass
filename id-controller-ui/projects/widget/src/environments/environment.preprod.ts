import { defaultEnv } from './default';

const appHost = 'https://my-advertising-experience.preprod.crto.in';

export const environment = {
  ...defaultEnv,
  production: true,
  idControllerAppUrl: appHost + '/open-pass',
};
