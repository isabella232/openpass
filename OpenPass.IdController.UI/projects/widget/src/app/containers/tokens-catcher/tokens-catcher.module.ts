import { Inject, NgModule } from '@angular/core';
import { WINDOW } from '@utils/injection-tokens';
import { DOCUMENT } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { PublicApiService } from '@services/public-api.service';
import { environment } from '@env';
import { CookiesService } from '@services/cookies.service';

@NgModule()
export class TokensCatcherModule {
  private appHost = environment.appHost;
  private brandDomain: string;

  constructor(
    @Inject(WINDOW) private window: Window,
    @Inject(DOCUMENT) private document: Document,
    private cookiesService: CookiesService,
    private translateService: TranslateService,
    private publicApiService: PublicApiService
  ) {
    this.brandDomain = this.window.location.host;
    const urlSearchParams = new URLSearchParams(this.window.location.search);
    if (urlSearchParams.has('uid2Token') && urlSearchParams.has('ifaToken') && urlSearchParams.has('email')) {
      this.saveTokens(urlSearchParams.get('uid2Token'), urlSearchParams.get('ifaToken'));
      this.showMessage(urlSearchParams.get('email'));
      this.clearSearchParams();
    }
  }

  private saveTokens(uid2Token: string, ifaToken: string) {
    this.cookiesService.setCookie(environment.cookieUid2Token, uid2Token, environment.cookieLifetimeDays);
    this.cookiesService.setCookie(environment.cookieIfaToken, ifaToken, environment.cookieLifetimeDays);
    this.publicApiService.setUserData({ uid2Token, ifaToken });
  }

  private showMessage(email: string) {
    const subscription = this.translateService
      .get('MESSAGES.SIGNED_IN', { appHost: this.appHost, email, brandDomain: this.brandDomain })
      .subscribe((message) => {
        const nativeElement = this.getSnackBar(message, { delay: '5000' });
        this.document.body.appendChild(nativeElement);

        subscription.unsubscribe();
      });
  }

  private getSnackBar(message: string, attributes: { [key: string]: string }) {
    const snackBar = this.document.createElement('wdgt-snack-bar');
    snackBar.innerHTML = message;
    for (const [attrName, attrValue] of Object.entries(attributes)) {
      snackBar.setAttribute(attrName, attrValue);
    }
    return snackBar;
  }

  private clearSearchParams() {
    const currentUrl = new URL(this.window.location.href);
    currentUrl.searchParams.delete('ifaToken');
    currentUrl.searchParams.delete('uid2Token');
    currentUrl.searchParams.delete('email');
    this.window.history.replaceState({}, this.document.title, currentUrl.toString());
  }
}
