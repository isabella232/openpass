const APP_PATH = '//localhost:4300';

(() => {
  const hash = new Date().getMilliseconds();
  const requireScript = (url) => {
    const script = document.createElement('script');
    script.src = `${APP_PATH}/${url}?ver=${hash}`;
    document.head.append(script);
  };

  ['polyfills.js', 'runtime.js', 'styles.js', 'vendor.js', 'main.js'].forEach(requireScript);
})();
