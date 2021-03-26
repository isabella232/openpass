import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TokenDto } from '../otp/token.dto';
import { environment } from '@env';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UnauthenticatedService {
  private readonly namespace = environment.namespace;

  constructor(private http: HttpClient) {}

  getIfa(): Observable<TokenDto> {
    return this.http.get<TokenDto>(this.namespace + '/unauthenticated');
  }
}
