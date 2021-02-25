import { Component, Inject, OnInit } from '@angular/core';
import { PostMessagesService } from '@services/post-messages.service';
import { Dispatch } from '@ngxs-labs/dispatch-decorator';
import { SaveOpener } from '@store/otp-widget/opener.actions';

@Component({
  selector: 'usrf-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private postMessageService: PostMessagesService, @Inject('Window') private window: Window) {}

  @Dispatch()
  private recognizeOrigin() {
    const searchParams = new URLSearchParams(this.window.location.search);
    const origin = searchParams.get('origin');
    return new SaveOpener(origin);
  }

  ngOnInit() {
    this.recognizeOrigin();
    this.postMessageService.startListening();
  }
}
