module.exports = [
  {
    context: ['/api'],
    target: 'https://my-advertising-experience.preprod.crto.in',
    changeOrigin: true,
    secure: true,
  },
];
