import { requireScript } from './utils/require-script';
import { environment } from './environment/environment';
import { attachCallback } from './app/attach-callback';

declare global {
  interface Window {
    __OpenPass: {
      signUserIn?: (email: string) => void;
    };
  }
}

environment.scriptNames.forEach(requireScript);

window.__OpenPass = window.__OpenPass || {};
window.__OpenPass.signUserIn = attachCallback;
