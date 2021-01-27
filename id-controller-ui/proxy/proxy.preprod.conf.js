module.exports = [
  {
    context: ['/api'],
    target: 'https://criteo.com/', // Change url to preprod
    changeOrigin: true,
    secure: true,
  },
];
