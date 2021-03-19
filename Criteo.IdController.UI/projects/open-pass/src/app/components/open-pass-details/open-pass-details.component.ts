import { Component, Input } from '@angular/core';

@Component({
  selector: 'usrf-open-pass-details',
  templateUrl: './open-pass-details.component.html',
  styleUrls: ['./open-pass-details.component.scss'],
})
export class OpenPassDetailsComponent {
  @Input()
  name: string;

  handleDetailsToggle({ target }: Event) {
    if ((target as HTMLDetailsElement).open) {
      (target as HTMLDetailsElement).scrollIntoView?.({ behavior: 'smooth', block: 'end' });
    }
  }
}
