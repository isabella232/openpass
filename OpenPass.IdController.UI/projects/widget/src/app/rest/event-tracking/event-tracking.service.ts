import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EventTypes } from '@shared/enums/event-types.enum';
import { environment } from '@env';
import { Observable } from 'rxjs';
import { WINDOW } from '@utils/injection-tokens';
import { WidgetConfigurationService } from '@services/widget-configuration.service';
import { mergeMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class EventTrackingService {
  constructor(
    private http: HttpClient,
    @Inject(WINDOW) private window: Window,
    private widgetConfigurationService: WidgetConfigurationService
  ) {}

  track(eventType: EventTypes): Observable<void> {
    return this.widgetConfigurationService.getConfiguration().pipe(
      mergeMap((config) =>
        this.http.post<void>(
          environment.appHost + '/api/event',
          { eventType },
          {
            headers: {
              'x-tracked-data': JSON.stringify(config),
              'x-origin-host': this.window.location.origin,
            },
          }
        )
      )
    );
  }
}
