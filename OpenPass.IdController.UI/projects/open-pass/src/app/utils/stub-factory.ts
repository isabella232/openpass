export const stub = (serviceClass: any, mock = {}) => ({
  provide: serviceClass,
  useFactory: () => mock,
});
