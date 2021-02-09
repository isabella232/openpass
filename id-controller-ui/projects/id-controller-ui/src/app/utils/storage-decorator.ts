const isStorageAvailable = (() => {
  try {
    window.localStorage.setItem('test-storage-item', '42');
    window.localStorage.removeItem('test-storage-item');
    return true;
  } catch (error) {
    return false;
  }
})();

export const localStorage = <T>(storagePath: string): ((target: any, propertyKey: string) => void) => {
  const STORAGE_PREFIX = 'USRF.';
  const fullPath = STORAGE_PREFIX + storagePath;

  if (!isStorageAvailable) {
    return (target: any, propertyKey: string) => {};
  }

  return (target: any, propertyKey: string): void => {
    Object.defineProperties(target, {
      [propertyKey]: {
        get: () => {
          const value = window.localStorage.getItem(fullPath) as string;
          try {
            return JSON.parse(value);
          } catch {
            return value;
          }
        },
        set: (value: T) => window.localStorage.setItem(fullPath, JSON.stringify(value)),
      },
    });
  };
};
