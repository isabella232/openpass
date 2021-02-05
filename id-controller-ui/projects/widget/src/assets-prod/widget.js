const APP_PATH = location.host.match(/\.preprod/) ?
  'https://my-advertising-experience.preprod.crto.in' :
  'https://my-advertising-experience.crto.in'; // TODO: Make it safer

(() => {
  const hash = new Date().getMilliseconds();
  const requireScript = (url) => {
    const script = document.createElement('script');
    script.src = `${APP_PATH}/${url}?ver=${hash}`;
    document.head.append(script);
  };
  requireScript('polyfills.js');
  requireScript('main.js');
})();
