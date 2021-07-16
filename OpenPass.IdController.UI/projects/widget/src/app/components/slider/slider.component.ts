import { AfterViewInit, Component, ContentChildren, ElementRef, Input, QueryList, TemplateRef } from '@angular/core';
import { Subject, timer } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'wdgt-slider',
  templateUrl: './slider.component.html',
  styleUrls: ['./slider.component.scss'],
})
export class SliderComponent implements AfterViewInit {
  @Input() delay = 5000;

  @ContentChildren(TemplateRef) slides: QueryList<TemplateRef<ElementRef>>;

  activeIndex = 0;
  private transition$ = new Subject<boolean>();

  ngAfterViewInit() {
    if (this.slides.length) {
      this.runCarousel();
    }
  }

  setActiveSlide(slideIndex: number) {
    this.transition$.next(true);
    this.activeIndex = slideIndex;
    this.runCarousel();
  }

  private runCarousel() {
    this.transition$.next(false);
    timer(this.delay)
      .pipe(takeUntil(this.transition$))
      .subscribe(() => this.setActiveSlide((this.activeIndex + 1) % this.slides.length));
  }
}
