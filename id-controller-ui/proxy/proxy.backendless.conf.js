const userMock = require('../cypress/fixtures/foo.json');

const mockedRoutes = new Map([
  [/api\/self$/, () => userMock], // example
  [/api\/users\/\d+$/, (id) => ({ ...userMock, id })], // example
  [/api\/generate$/, () => ''],
  [/api\/validate$/, () => ''],
]);

module.exports = [
  {
    context: ['/api'],
    target: 'https://my-advertising-experience.preprod.crto.in',
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
