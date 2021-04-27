import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[wdgtViewContainer]',
})
export class ViewContainerDirective {
  constructor(public viewContainerRef: ViewContainerRef) {}
}
