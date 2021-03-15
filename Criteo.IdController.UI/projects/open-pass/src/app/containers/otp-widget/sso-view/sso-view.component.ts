import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { GapiService } from '@services/gapi.service';

@Component({
  selector: 'usrf-sso-view',
  templateUrl: './sso-view.component.html',
  styleUrls: ['./sso-view.component.scss'],
})
export class SsoViewComponent implements OnInit, AfterViewInit {
  @ViewChild('googleButton')
  googleButton: ElementRef;

  constructor(private gapiService: GapiService) {}

  ngOnInit(): void {
    this.gapiService.load();
  }

  ngAfterViewInit() {
    this.gapiService.renderButton(this.googleButton.nativeElement);
  }
}
