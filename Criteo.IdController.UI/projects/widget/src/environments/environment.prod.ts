import { defaultEnv } from './default';

const appHost = 'https://openpass.criteo.com';

export const environment = {
  ...defaultEnv,
  production: true,
  idControllerAppUrl: appHost + '/open-pass',
};
