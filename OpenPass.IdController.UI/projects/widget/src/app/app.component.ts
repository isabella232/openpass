import { Component, ComponentFactoryResolver, OnInit, ViewChild } from '@angular/core';
import { ViewContainerDirective } from '@directives/view-container.directive';

@Component({
  selector: 'wdgt-app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  @ViewChild(ViewContainerDirective, { static: true })
  private viewElement: ViewContainerDirective;

  private widgetsList = ['SnackBarComponent', 'LandingComponent', 'IdentificationComponent'];

  constructor(private componentFactoryResolver: ComponentFactoryResolver) {}

  async ngOnInit() {
    // eslint-disable-next-line @typescript-eslint/naming-convention
    const { DevComponentsComponent } = await import('./components/dev-components/dev-components.component');
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(DevComponentsComponent);
    const componentRef = this.viewElement.viewContainerRef.createComponent(componentFactory);
    componentRef.instance.items = this.widgetsList;
  }
}
