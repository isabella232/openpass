import { Component } from '@angular/core';
import { Route, Router } from '@angular/router';
import { DialogWindowService } from '../../services/dialog-window.service';
import { EventTypes } from '../../enums/event-types.enum';
import { EventsTrackingService } from '../../services/events-tracking.service';

@Component({
  selector: 'usrf-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent {
  get hasBackHistory() {
    return !this.router.config.map((route: Route) => '/' + route.path).includes(this.router.url.replace(/\?.+/, ''));
  }

  constructor(
    private router: Router,
    private dialogWindowService: DialogWindowService,
    private eventsTrackingService: EventsTrackingService
  ) {}

  close() {
    this.eventsTrackingService.trackEvent(EventTypes.bannerIgnored);
    this.dialogWindowService.closeDialogWindow();
  }

  back() {
    this.router.navigate(['..']);
  }
}
