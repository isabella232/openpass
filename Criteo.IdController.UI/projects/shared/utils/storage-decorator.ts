const STORE_PATH = 'USRF';
const isStorageCreated = (() => {
  try {
    window.localStorage.setItem(STORE_PATH, window.localStorage.getItem(STORE_PATH) ?? '{}');
    return true;
  } catch (error) {
    return false;
  }
})();

export const localStorage = <T>(storagePath: string): ((target: any, propertyKey: string) => void) => {
  if (!isStorageCreated) {
    return (target: any, propertyKey: string) => {};
  }

  return (target: any, propertyKey: string): void => {
    Object.defineProperties(target, {
      [propertyKey]: {
        get: () => {
          const store = JSON.parse(window.localStorage.getItem(STORE_PATH));
          return chainGet(store, storagePath);
        },
        set: (value: T) => {
          let store = JSON.parse(window.localStorage.getItem(STORE_PATH));
          store = chainSet(store, storagePath, value);
          window.localStorage.setItem(STORE_PATH, JSON.stringify(store));
        },
      },
    });
  };
};

/**
 * Function that sets a value to an object along the chain path.
 * Ex: 'user.address.street' => {user: {address: {street: value}}}
 * @param storage - object
 * @param path - chain string ('user.address.street')
 * @param value - any value
 */
function chainSet(storage: { [p: string]: any }, path: string, value: any): { [p: string]: any } {
  return path.split('.').reduce(
    ([root, prev], current, index, array) => {
      const targetObj = prev ?? root;
      targetObj[current] = index + 1 !== array.length ? targetObj[current] ?? {} : value;
      return [root, targetObj[current]];
    },
    storage ? [storage] : [{}]
  )[0];
}

/**
 * Function that returns a value from an object reaching it along the chain path
 * @param storage - object
 * @param path - chain string ('user.address.street')
 */
function chainGet(storage: { [key: string]: any }, path: string): any {
  return path.split('.').reduce((prev, current): any => prev?.[current], storage ?? {});
}
