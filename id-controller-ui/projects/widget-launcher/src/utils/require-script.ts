import { environment } from '../environment/environment';

let hash: number = new Date().getMilliseconds();
export function requireScript(scriptName: string) {
  let es5 = document.createElement('script');
  let es2015 = document.createElement('script');
  if (!environment.isDevelopment) {
    es5.src = `${environment.APP_PATH}/${scriptName}-es5.js?ver=${hash}`;
    es5.defer = es5.noModule = true;
    es5.async = false;
    es2015.src = `${environment.APP_PATH}/${scriptName}-es2015.js?ver=${hash}`;
    es2015.async = false;
    es2015.type = 'module';
    document.head.appendChild(es5);
  } else {
    es2015.src = `${environment.APP_PATH}/${scriptName}.js`;
  }
  document.head.appendChild(es2015);
}
