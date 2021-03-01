import { Injectable } from '@angular/core';
import { UserData } from '@shared/types/public-api/user-data';
import { localStorage } from '@shared/utils/storage-decorator';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PublicApiService {
  @localStorage('widget.token') token: string;
  @localStorage('widget.email') email: string;
  @localStorage('widget.isDeclined') isDeclined: boolean;

  private userData = new Subject<UserData>();

  getSubscription() {
    return this.userData.asObservable();
  }

  setUserData({ token, email, isDeclined }: UserData) {
    this.token = token;
    this.email = email;
    this.isDeclined = isDeclined;
    this.userData.next({ token, email, isDeclined });
  }

  getUserData(): UserData {
    const { token, email, isDeclined = false } = this;
    return { token, email, isDeclined };
  }
}
