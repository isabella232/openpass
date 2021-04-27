import { defaultEnv } from './default';

const appHost = 'https://openpass.preprod.criteo.com';

export const environment = {
  ...defaultEnv,
  production: true,
  idControllerAppUrl: appHost + '/open-pass',
};
