import { Injectable } from '@angular/core';
import { environment } from '@env';
import { HttpClient } from '@angular/common/http';
import { OtpDto } from '../otp/otp.dto';
import { Observable } from 'rxjs';
import { TokenDto } from '../otp/token.dto';

@Injectable({
  providedIn: 'root',
})
export class AuthenticatedService {
  private readonly namespace = environment.namespace;

  constructor(private http: HttpClient) {}

  generateOtp(otp: OtpDto): Observable<void> {
    return this.http.post<void>(this.namespace + '/authenticated/otp/generate', otp);
  }

  validateOtp(otp: OtpDto): Observable<TokenDto> {
    return this.http.post<TokenDto>(this.namespace + '/authenticated/otp/validate', otp);
  }

  getTokenByEmail(email: string): Observable<TokenDto> {
    return this.http.post<TokenDto>(this.namespace + '/authenticated/sso', { email });
  }
}
