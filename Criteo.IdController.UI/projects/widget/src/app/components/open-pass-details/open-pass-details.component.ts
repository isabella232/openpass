import { Component } from '@angular/core';

@Component({
  selector: 'wdgt-open-pass-details',
  templateUrl: './open-pass-details.component.html',
  styleUrls: ['./open-pass-details.component.scss'],
})
export class OpenPassDetailsComponent {
  handleDetailsToggle({ target }: Event) {
    if ((target as HTMLDetailsElement).open) {
      // TODO: run only for modal?
      // (target as HTMLDetailsElement).scrollIntoView?.({ behavior: 'smooth', block: 'end' });
    }
  }
}
