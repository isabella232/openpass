import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EventTypes } from '@shared/enums/event-types.enum';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { WINDOW } from '../../utils/injection-tokens';

@Injectable({
  providedIn: 'root',
})
export class EventTrackingService {
  constructor(private http: HttpClient, @Inject(WINDOW) private window: Window) {}

  track(eventType: EventTypes): Observable<void> {
    const originHost = this.window.location.origin;
    return this.http.post<void>(
      environment.appHost + '/api/event',
      { eventType },
      { headers: { 'x-origin-host': originHost } }
    );
  }
}
