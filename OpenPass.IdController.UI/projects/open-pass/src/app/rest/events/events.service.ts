import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env';
import { EventTypes } from '@shared/enums/event-types.enum';

@Injectable({
  providedIn: 'root',
})
export class EventsService {
  private readonly namespace = environment.namespace;

  constructor(private http: HttpClient) {}

  trackEvent(eventType: EventTypes): Observable<void> {
    return this.http.post<void>(this.namespace + '/event', { eventType });
  }
}
