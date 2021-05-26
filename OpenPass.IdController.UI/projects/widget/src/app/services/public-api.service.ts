import { Injectable } from '@angular/core';
import { UserData } from '@shared/types/public-api/user-data';
import { localStorage } from '@shared/utils/storage-decorator';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PublicApiService {
  @localStorage('widget.ifaToken') ifaToken: string;
  @localStorage('widget.uid2Token') uid2Token: string;
  @localStorage('widget.isDeclined') isDeclined: boolean;

  private userData = new Subject<UserData>();

  getSubscription() {
    return this.userData.asObservable();
  }

  setUserData({ ifaToken = this.ifaToken, uid2Token = this.uid2Token, isDeclined }: UserData) {
    this.ifaToken = ifaToken;
    this.uid2Token = uid2Token;
    this.isDeclined = isDeclined;
    this.userData.next({ ifaToken, uid2Token, isDeclined });
  }

  getUserData(): UserData {
    const { ifaToken, uid2Token, isDeclined = false } = this;
    return { ifaToken, uid2Token, isDeclined };
  }
}
