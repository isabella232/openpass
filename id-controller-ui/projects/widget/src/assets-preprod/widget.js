!function (doc) {
  const APP_PATH = 'https://my-advertising-experience.preprod.crto.in/open-pass/preprod/widget';
  const hash = new Date().getMilliseconds(), t = 'script';

  function require(sn) {
    const es5 = doc.createElement(t);
    es5.src = APP_PATH + '/' + sn + '-es5.js?ver=' + hash;
    es5.defer = es5.noModule = true;
    es5.async = false;
    doc.head.appendChild(es5);
    const es2015 = doc.createElement(t);
    es2015.src = APP_PATH + '/' + sn + '-es2015.js?ver=' + hash;
    es2015.async = false;
    es2015.type = 'module';
    doc.head.appendChild(es2015);
  }

  ['polyfills', 'main'].forEach(require);
}(document);
