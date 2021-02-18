import { Inject, Injectable } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class CookiesService {
  constructor(@Inject(DOCUMENT) private document: Document) {}

  getCookie(name: string) {
    const cookies = this.getCookiesMap();
    if (cookies.hasOwnProperty(name)) {
      try {
        return JSON.parse(cookies[name]);
      } catch {
        return cookies[name];
      }
    }
    return undefined;
  }

  resetCookie(name: string) {
    this.document.cookie = name + '=; Max-Age=-1;';
  }

  private getCookiesMap(): { [key: string]: string } {
    const parsedCookies = this.document.cookie.split('; ');
    const cookiesMap = parsedCookies.map((cookieString) => cookieString.split('='));
    return cookiesMap.reduce((obj: { [key: string]: string }, [key, value]) => {
      obj[key] = value;
      return obj;
    }, {});
  }
}
