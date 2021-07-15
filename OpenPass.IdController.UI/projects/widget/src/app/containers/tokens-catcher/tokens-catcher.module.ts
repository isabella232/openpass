import { ComponentFactoryResolver, Inject, Injector, NgModule } from '@angular/core';
import { WINDOW } from '@utils/injection-tokens';
import { DOCUMENT } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { PublicApiService } from '@services/public-api.service';
import { SnackBarComponent } from '@components/snack-bar/snack-bar.component';
import { environment } from '@env';
import { CookiesService } from '@services/cookies.service';

@NgModule()
export class TokensCatcherModule {
  private appHost = environment.appHost;
  private brandDomain: string;

  constructor(
    @Inject(WINDOW) private window: Window,
    @Inject(DOCUMENT) private document: Document,
    private resolver: ComponentFactoryResolver,
    private injector: Injector,
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
    const componentFactory = this.resolver.resolveComponentFactory(SnackBarComponent);
    const subscription = this.translateService
      .get('MESSAGES.SIGNED_IN', { appHost: this.appHost, email, brandDomain: this.brandDomain })
      .subscribe((message) => {
        const ngContent = [this.getContentMessage(message)];
        const componentRef = componentFactory.create(this.injector, ngContent);
        componentRef.instance.delay = 6000;
        componentRef.changeDetectorRef.detectChanges();

        const { nativeElement } = componentRef.location;
        this.document.body.appendChild(nativeElement);

        subscription.unsubscribe();
      });
  }

  private getContentMessage(content: string) {
    const div = this.document.createElement('div');
    div.insertAdjacentHTML('afterbegin', content);
    return Array.from(div.childNodes);
  }

  private clearSearchParams() {
    const currentUrl = new URL(this.window.location.href);
    currentUrl.searchParams.delete('ifaToken');
    currentUrl.searchParams.delete('uid2Token');
    currentUrl.searchParams.delete('email');
    this.window.history.replaceState({}, this.document.title, currentUrl.toString());
  }
}
