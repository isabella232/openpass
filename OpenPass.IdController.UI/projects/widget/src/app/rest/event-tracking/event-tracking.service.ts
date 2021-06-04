import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EventTypes } from '@shared/enums/event-types.enum';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { WINDOW } from '../../utils/injection-tokens';
import { WidgetConfigurationService } from '../../services/widget-configuration.service';
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
      mergeMap((config) => {
        const originHost = this.window.location.origin;
        const trackedData = {
          originHost,
          view: config.view,
          variant: config.variant,
          session: config.session,
          provider: config.provider,
        };
        return this.http.post<void>(
          environment.appHost + '/api/event',
          { eventType },
          { headers: { 'x-tracked-data': JSON.stringify(trackedData) } }
        );
      })
    );
  }
}
