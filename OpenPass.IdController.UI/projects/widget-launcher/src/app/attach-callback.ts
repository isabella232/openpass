import { environment } from '../environment/environment';

export const attachCallback = (email: string) => {
  const url = new URL(environment.openPassAppPath + '/redirect');
  const origin = window.location.href;
  url.searchParams.set('origin', origin);
  url.searchParams.set('email', email);
  window.location.assign(url.toString());
};
