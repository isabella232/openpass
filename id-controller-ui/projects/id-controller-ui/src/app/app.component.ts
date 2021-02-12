import { Component, Inject, OnInit } from '@angular/core';
import { PostMessagesService } from '@services/post-messages.service';

@Component({
  selector: 'usrf-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private postMessageService: PostMessagesService, @Inject('Window') private window: Window) {}

  ngOnInit() {
    const searchParams = new URLSearchParams(this.window.location.search);
    const origin = searchParams.get('origin');
    if (origin) {
      this.postMessageService.startListing(origin);
    }
  }
}
