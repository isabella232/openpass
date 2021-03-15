import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { from, Observable, of } from 'rxjs';
import { map, mergeAll } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class GetScriptsResolver implements Resolve<boolean> {
  private resolvedUrls = new Set<string>();

  constructor(@Inject(DOCUMENT) private document: Document) {}

  resolve(route: ActivatedRouteSnapshot): Observable<boolean> {
    if (!route.routeConfig.data.preloadScripts?.length) {
      return of(true);
    }
    return from<string[]>(route.routeConfig.data.preloadScripts ?? []).pipe(
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
}
