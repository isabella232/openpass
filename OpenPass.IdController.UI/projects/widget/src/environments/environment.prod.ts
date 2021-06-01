import { defaultEnv } from './default';

const appHost = 'https://openpass.criteo.com';

export const environment = {
  appHost,
  ...defaultEnv,
  production: true,
  idControllerAppUrl: appHost + '/open-pass',
};
