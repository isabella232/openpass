import { NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { from, Observable } from 'rxjs';

export class CustomTranslateLoader implements TranslateLoader {
  getTranslation(lang: string): Observable<any> {
    return from(import(`../../../assets/i18n/${lang}.json`));
  }
}

@NgModule({
  declarations: [],
  imports: [
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useClass: CustomTranslateLoader,
      },
    }),
  ],
})
export class TranslationModule {
  constructor(private translateService: TranslateService) {
    this.applyLanguage();
  }

  private applyLanguage() {
    const supportedLanguages = ['en', 'ja'];
    const userLanguage = this.translateService.getBrowserLang();
    if (supportedLanguages.includes(userLanguage) && userLanguage !== this.translateService.getDefaultLang()) {
      this.translateService.use(userLanguage);
    }
  }
}
