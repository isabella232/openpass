import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
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

  isClosing = false;
  isAppearing = false;

  constructor(private elementRef: ElementRef) {}

  ngOnInit() {
    timer(100).subscribe(() => (this.isAppearing = true));
    if (this.delay) {
      timer(this.delay).subscribe(() => this.close());
    }
  }

  close() {
    this.closeClick.emit();
    this.isClosing = true;
    timer(500).subscribe(() => this.elementRef.nativeElement.remove());
  }
}
