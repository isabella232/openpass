import { Inject, Pipe, PipeTransform } from '@angular/core';
import { DEPLOY_URL } from '../utils/injection-tokens';

@Pipe({
  name: 'useDeployUrl',
})
export class UseDeployUrlPipe implements PipeTransform {
  constructor(@Inject(DEPLOY_URL) private deployUrl: string) {}

  transform(url: string): string {
    return this.deployUrl + url;
  }
}
