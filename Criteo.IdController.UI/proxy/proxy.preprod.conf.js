module.exports = [
  {
    context: ['/api'],
    target: 'https://openpass.preprod.criteo.com',
    changeOrigin: true,
    secure: true,
  },
];
