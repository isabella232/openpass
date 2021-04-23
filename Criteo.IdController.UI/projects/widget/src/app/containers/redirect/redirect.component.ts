import { Component, Inject, Input, NgModule, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Sessions } from '../../enums/sessions.enum';
import { WINDOW } from '../../utils/injection-tokens';
import { CookiesService } from '../../services/cookies.service';
import { PublicApiService } from '../../services/public-api.service';

@Component({ template: '' })
export class RedirectComponent implements OnInit {
  @Input() session: Sessions;

  constructor(
    @Inject(WINDOW) private window: Window,
    private cookiesService: CookiesService,
    private publicApiService: PublicApiService
  ) {}

  ngOnInit() {
    const hasCookie = !!this.cookiesService.getCookie(environment.cookieName);
    const token = new URL(this.window.location.href).searchParams.get('token');

    if (!hasCookie) {
      if (token) {
        this.saveToken(token);
      } else {
        this.doRedirect();
      }
    }
  }

  private doRedirect() {
    const destUrl = new URL(environment.idControllerAppUrl);
    destUrl.searchParams.set('origin', this.window.location.href);
    if (this.session === Sessions.unauthenticated) {
      destUrl.pathname += environment.unloggedPath;
    }

    this.window.location.replace(destUrl.toString());
  }

  private saveToken(token: string) {
    this.cookiesService.setCookie(environment.cookieName, token, environment.cookieLifetimeDays);
    this.publicApiService.setUserData({ token });

    const clearPath = new URL(this.window.location.href);
    clearPath.searchParams.delete('token');
    this.window.history.replaceState({}, this.window.document.title, clearPath.toString());
  }
}

@NgModule({
  declarations: [RedirectComponent],
  imports: [],
})
class RedirectModule {}
