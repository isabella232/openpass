import {
  Component,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewEncapsulation,
  EventEmitter,
} from '@angular/core';
import { DomSanitizer, SafeResourceUrl, SafeStyle } from '@angular/platform-browser';
import { UseDeployUrlPipe } from '../../pipes/use-deploy-url.pipe';
import { detectDarkColor } from '../../utils/ui-tools';
import { CookiesService } from '../../services/cookies.service';
import { PublicApiService } from '../../services/public-api.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'wdgt-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['../../../../../open-pass/src/styles.scss', './landing.component.scss'],
  providers: [UseDeployUrlPipe],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class LandingComponent implements OnInit, OnChanges {
  @Input()
  brandImageUrl: string;
  @Input()
  brandColor: string;

  @Output()
  requestSignin = new EventEmitter();
  @Output()
  requestSignup = new EventEmitter();

  isOpen = true;
  isBrandColorDark = true;
  safeBrandColor: SafeStyle;
  safeBrandImage: SafeResourceUrl;
  readonly appHost = environment.idControllerAppUrl;
  private readonly defaultLogoUrl = '/assets/images/default-customer-logo.png';
  private readonly defaultBrandColor = '#6C63FF';

  constructor(
    private sanitizer: DomSanitizer,
    private deployUrl: UseDeployUrlPipe,
    private cookiesService: CookiesService,
    private publicApiService: PublicApiService
  ) {}

  ngOnInit(): void {
    const hasCookie = !!this.cookiesService.getCookie(environment.cookieUid2Token);
    const { isDeclined } = this.publicApiService.getUserData();
    this.isOpen = !hasCookie && !isDeclined;
    this.setLogo();
    this.setColors();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.brandImageUrl?.firstChange === false) {
      this.setLogo();
    }
    if (changes.brandColor?.firstChange === false) {
      this.setColors();
    }
  }

  close() {
    this.isOpen = false;
    this.publicApiService.setUserData({ ifaToken: null, uid2Token: null, isDeclined: true });
  }

  signin() {
    this.requestSignin.emit();
  }

  signup() {
    this.requestSignup.emit();
  }

  private setColors() {
    this.safeBrandColor = this.brandColor
      ? this.sanitizer.bypassSecurityTrustStyle(this.brandColor)
      : this.defaultBrandColor;
    this.isBrandColorDark = detectDarkColor(this.brandColor ?? this.defaultBrandColor);
  }

  private setLogo() {
    const defaultBrandImage = this.deployUrl.transform(this.defaultLogoUrl);
    const url = this.brandImageUrl ?? defaultBrandImage;
    this.safeBrandImage = this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }
}
