import { State } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { OpenerState } from './opener.state';

@State<void>({
  name: 'otpWidget',
  children: [OpenerState],
})
@Injectable()
export class OtpWidgetState {}
