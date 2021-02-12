import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OtpDto } from './otp.dto';
import { Observable } from 'rxjs';
import { environment } from '@env';

@Injectable({
  providedIn: 'root',
})
export class OtpService {
  private readonly namespace = environment.namespace;

  constructor(private http: HttpClient) {}

  generateOtp(otp: OtpDto): Observable<void> {
    return this.http.post<void>(this.namespace + '/otp/generate', otp);
  }

  validateOtp(otp: OtpDto): Observable<void> {
    return this.http.post<void>(this.namespace + '/otp/validate', otp);
  }
}
