import { State } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { OpenerState } from './opener.state';
import { AuthState } from './auth.state';

@State<void>({
  name: 'otpWidget',
  children: [OpenerState, AuthState],
})
@Injectable()
export class OtpWidgetState {}
