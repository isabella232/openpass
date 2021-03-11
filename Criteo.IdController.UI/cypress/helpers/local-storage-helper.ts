export class LocalStorageHelper {
  static setFakeToken() {
    const current = JSON.parse(window.localStorage.getItem('USRF'));
    const patched = mergeDeep(current ?? {}, { openpass: { token: 'fake_token' } });
    LocalStorageHelper.setItem('USRF', JSON.stringify(patched));
  }

  static patchLocalStorage(rewrite: { [p: string]: any }) {
    const current = JSON.parse(window.localStorage.getItem('USRF'));
    const patched = mergeDeep(current ?? {}, rewrite);
    LocalStorageHelper.setItem('USRF', JSON.stringify(patched));
  }

  static clearLocalStorageItem(name: string) {
    cy.clearLocalStorage(name);
  }

  static clearLocalStorage() {
    cy.clearLocalStorage();
  }

  static setItem(name: string, value: string) {
    window.localStorage.setItem(name, value);
  }
}

const isObject = (item) => item && typeof item === 'object' && !Array.isArray(item);

function mergeDeep(target, ...sources) {
  if (!sources.length) return target;
  const source = sources.shift();

  if (isObject(target) && isObject(source)) {
    for (const key in source) {
      if (isObject(source[key])) {
        if (!target[key]) Object.assign(target, { [key]: {} });
        mergeDeep(target[key], source[key]);
      } else {
        Object.assign(target, { [key]: source[key] });
      }
    }
  }

  return mergeDeep(target, ...sources);
}
