import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OtpDto } from './otp.dto';
import { Observable } from 'rxjs';
import { environment } from '@env';
import { TokenDto } from './token.dto';

@Injectable({
  providedIn: 'root',
})
export class OtpService {
  private readonly namespace = environment.namespace;

  constructor(private http: HttpClient) {}

  generateOtp(otp: OtpDto): Observable<void> {
    return this.http.post<void>(this.namespace + '/authenticated/otp/generate', otp);
  }

  validateOtp(otp: OtpDto): Observable<TokenDto> {
    return this.http.post<TokenDto>(this.namespace + '/authenticated/otp/validate', otp);
  }

  getIfa(): Observable<TokenDto> {
    return this.http.get<TokenDto>(this.namespace + '/unauthenticated');
  }
}
