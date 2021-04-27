import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { from, Observable, of } from 'rxjs';
import { map, mergeAll } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root',
})
export class GetScriptsResolver implements Resolve<boolean> {
  private resolvedUrls = new Set<string>();

  private get cultureLang(): string {
    const currentLang = this.translate.getBrowserCultureLang();
    if (currentLang.includes('-')) {
      return currentLang.replace('-', '_');
    }

    // @ts-ignore
    if (Intl?.Locale) {
      // @ts-ignore
      return `${currentLang}_${new Intl.Locale(currentLang).maximize().region}`;
    }

    return 'en_US';
  }

  constructor(@Inject(DOCUMENT) private document: Document, private translate: TranslateService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<boolean> {
    if (!route.routeConfig.data.preloadScripts?.length) {
      return of(true);
    }
    return from<string[]>(route.routeConfig.data.preloadScripts ?? []).pipe(
      map((url: string) => this.parseUrl(url)),
      map((url: string) => this.scriptLoader(url)),
      mergeAll()
    );
  }

  private scriptLoader(url: string): Observable<boolean> {
    if (this.resolvedUrls.has(url)) {
      return of(true);
    }
    this.resolvedUrls.add(url);
    const script = this.document.createElement('script');
    script.async = true;
    script.src = url;
    script.type = 'text/javascript';

    return from(
      new Promise<boolean>((resolve) => {
        script.onload = () => resolve(true);
        this.document.head.appendChild(script);
      })
    );
  }

  private parseUrl(url: string) {
    return url.replace(/{{(\S*)}}/gi, (substring, ...args: any[]) => this.getValue(args[0]));
  }

  private getValue(variable: string): string {
    const variablesMap = {
      browserLang: this.cultureLang,
    };
    // @ts-ignore
    return variablesMap[variable];
  }
}
