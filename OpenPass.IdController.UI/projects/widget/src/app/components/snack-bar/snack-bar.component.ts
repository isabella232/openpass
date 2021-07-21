import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  NgModule,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { timer } from 'rxjs';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { CommonModule } from '@angular/common';
import { PipesModule } from '@pipes/pipes.module';

@UntilDestroy()
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
    timer(100)
      .pipe(untilDestroyed(this))
      .subscribe(() => {
        this.isAppearing = true;
        this.cd.detectChanges();
      });
    if (this.delay) {
      timer(this.delay)
        .pipe(untilDestroyed(this))
        .subscribe(() => this.close());
    }
  }

  close() {
    this.closeClick.emit();
    this.isAppearing = false;
    this.cd.detectChanges();
    timer(500)
      .pipe(untilDestroyed(this))
      .subscribe(() => this.elementRef.nativeElement.remove());
  }
}

@NgModule({
  declarations: [SnackBarComponent],
  imports: [CommonModule, PipesModule],
  exports: [SnackBarComponent],
})
class SnackBarModule {}
