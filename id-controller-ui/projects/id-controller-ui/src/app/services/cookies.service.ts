import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CookiesService {
  constructor(@Inject('Window') private window: Window) {}

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

  private getCookiesMap(): { [key: string]: string } {
    const parsedCookies = this.window.document.cookie.split('; ');
    const cookiesMap = parsedCookies.map((cookieString) => cookieString.split('='));
    return cookiesMap.reduce((obj: { [key: string]: string }, [key, value]) => {
      obj[key] = value;
      return obj;
    }, {});
  }
}
