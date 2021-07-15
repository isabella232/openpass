import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { timer } from 'rxjs';

@Component({
  selector: 'wdgt-snack-bar',
  templateUrl: './snack-bar.component.html',
  styleUrls: ['./snack-bar.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class SnackBarComponent implements OnInit {
  @Output()
  closeClick = new EventEmitter();
  @Input()
  delay: number;

  isAppearing = false;

  constructor(private elementRef: ElementRef, private cd: ChangeDetectorRef) {}

  ngOnInit() {
    timer(100).subscribe(() => {
      this.isAppearing = true;
      this.cd.detectChanges();
    });
    if (this.delay) {
      timer(this.delay).subscribe(() => this.close());
    }
  }

  close() {
    this.closeClick.emit();
    this.isAppearing = false;
    this.cd.detectChanges();
    timer(500).subscribe(() => this.elementRef.nativeElement.remove());
  }
}
