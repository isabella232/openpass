const WEBCOMPONENT_PATH = '//localhost:4300';

(() => {
  const hash = new Date().getMilliseconds();
  const requireScript = (url) => {
    const script = document.createElement('script');
    script.src = `${WEBCOMPONENT_PATH}/${url}?ver=${hash}`;
    document.head.append(script);
  };

  ['main.js', 'polyfills.js', 'runtime.js', 'styles.js', 'vendor.js'].forEach(requireScript);
})();
