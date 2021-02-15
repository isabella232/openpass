const APP_PATH = 'https://my-advertising-experience.crto.in/open-pass/widget';

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
