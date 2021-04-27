import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env';
import { EventDto } from './event.dto';

@Injectable({
  providedIn: 'root',
})
export class EventsService {
  private readonly namespace = environment.namespace;

  constructor(private http: HttpClient) {}

  trackEvent(event: EventDto): Observable<void> {
    return this.http.post<void>(this.namespace + '/event', event);
  }
}
