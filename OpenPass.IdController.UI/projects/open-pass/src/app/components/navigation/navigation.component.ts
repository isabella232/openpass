import { Component } from '@angular/core';
import { Route, Router } from '@angular/router';
import { DialogWindowService } from '@services/dialog-window.service';
import { EventTypes } from '@shared/enums/event-types.enum';
import { EventsService } from '@rest/events/events.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
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
    private eventsTrackingService: EventsService
  ) {}

  close() {
    this.eventsTrackingService
      .trackEvent(EventTypes.bannerIgnored)
      .pipe(untilDestroyed(this))
      .subscribe(() => this.dialogWindowService.closeDialogWindow());
  }

  back() {
    this.router.navigate(['..']);
  }
}
