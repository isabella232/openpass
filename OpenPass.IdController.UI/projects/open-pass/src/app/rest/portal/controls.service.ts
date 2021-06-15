import { Injectable } from '@angular/core';
import { environment } from '@env';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ControlsService {
  private readonly namespace = environment.namespace;

  constructor(private http: HttpClient) {}

  optOut(): Observable<void> {
    return this.http.post<void>(this.namespace + '/portal/controls/optout', {});
  }

  optIn(): Observable<void> {
    return this.http.post<void>(this.namespace + '/portal/controls/optin', {});
  }
}
