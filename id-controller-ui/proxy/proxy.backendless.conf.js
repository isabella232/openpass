const userMock = require('../cypress/fixtures/foo.json');

const mockedRoutes = new Map([
  [/api\/self$/, () => userMock], // example
  [/api\/users\/\d+$/, (id) => ({ ...userMock, id })], // example
]);

module.exports = [
  {
    context: ['/api'],
    target: 'https://criteo.com/', // Change url to preprod
    changeOrigin: true,
    secure: true,
    bypass: (req, res) => {
      for (let [regex, response] of mockedRoutes) {
        if (regex.test(req.url)) {
          let lastNumber;
          if (regex.toString().indexOf('d+') > 0) {
            const matches = req.url.match(/(\d+)/g);
            lastNumber = matches[matches.length - 1];
          }
          return res.json(response(lastNumber));
        }
      }
    },
  },
];
