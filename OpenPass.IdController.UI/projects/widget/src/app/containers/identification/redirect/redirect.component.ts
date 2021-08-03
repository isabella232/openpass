import { Component, Inject, NgModule, OnInit } from '@angular/core';
import { environment } from '@env';
import { Sessions } from '@enums/sessions.enum';
import { WINDOW } from '@utils/injection-tokens';
import { CookiesService } from '@services/cookies.service';
import { PublicApiService } from '@services/public-api.service';
import { WidgetConfigurationService } from '@services/widget-configuration.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({ template: '' })
export class RedirectComponent implements OnInit {
  constructor(
    @Inject(WINDOW) private window: Window,
    private cookiesService: CookiesService,
    private publicApiService: PublicApiService,
    private widgetConfigurationService: WidgetConfigurationService
  ) {}

  ngOnInit() {
    const hasCookie =
      !!this.cookiesService.getCookie(environment.cookieUid2Token) ||
      !!this.cookiesService.getCookie(environment.cookieIfaToken);
    const searchParams = new URL(this.window.location.href).searchParams;
    const hasToken = searchParams.has('uid2Token') || searchParams.has('ifaToken');
    const uid2Token = searchParams.get('uid2Token');
    const ifaToken = searchParams.get('ifaToken');
    const { isDeclined } = this.publicApiService.getUserData();

    if (!hasCookie) {
      if (hasToken) {
        if (uid2Token || ifaToken) {
          this.saveToken(ifaToken, uid2Token);
        } else {
          this.saveDecline();
        }
      } else if (!isDeclined) {
        this.doRedirect();
      }
    }
  }

  private doRedirect() {
    this.widgetConfigurationService
      .getConfiguration()
      .pipe(untilDestroyed(this))
      .subscribe((config) => {
        const destUrl = new URL(environment.idControllerAppUrl);
        const queryParams = new URLSearchParams({ origin: this.window.location.href, ...config });
        if (config.session === Sessions.unauthenticated) {
          destUrl.pathname += environment.unloggedPath;
        }
        this.window.location.replace(destUrl.toString() + '?' + queryParams.toString());
      });
  }

  private saveToken(ifaToken: string, uid2Token: string) {
    this.cookiesService.setCookie(environment.cookieUid2Token, uid2Token, environment.cookieLifetimeDays);
    this.cookiesService.setCookie(environment.cookieIfaToken, ifaToken, environment.cookieLifetimeDays);
    this.publicApiService.setUserData({ ifaToken, uid2Token });
    this.clearUrl();
  }

  private saveDecline() {
    this.publicApiService.setUserData({ isDeclined: true });
    this.clearUrl();
  }

  private clearUrl() {
    const clearPath = new URL(this.window.location.href);
    clearPath.searchParams.delete('ifaToken');
    clearPath.searchParams.delete('uid2Token');
    this.window.history.replaceState({}, this.window.document.title, clearPath.toString());
  }
}

@NgModule({
  declarations: [RedirectComponent],
  imports: [],
})
class RedirectModule {}
