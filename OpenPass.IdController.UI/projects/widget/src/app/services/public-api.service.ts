import { Injectable } from '@angular/core';
import { UserData } from '@shared/types/public-api/user-data';
import { localStorage } from '@shared/utils/storage-decorator';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PublicApiService {
  @localStorage('widget.token') token: string;
  @localStorage('widget.isDeclined') isDeclined: boolean;

  private userData = new Subject<UserData>();

  getSubscription() {
    return this.userData.asObservable();
  }

  setUserData({ token, isDeclined }: UserData) {
    this.token = token;
    this.isDeclined = isDeclined;
    this.userData.next({ token, isDeclined });
  }

  getUserData(): UserData {
    const { token, isDeclined = false } = this;
    return { token, isDeclined };
  }
}
