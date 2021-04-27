import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { DialogWindowService } from '../../services/dialog-window.service';

@Component({
  selector: 'usrf-reset',
  template: '',
})
export class ResetComponent implements OnInit {
  constructor(private authService: AuthService, private dialogWindowService: DialogWindowService) {}

  ngOnInit() {
    this.authService.resetToken();
    this.dialogWindowService.closeDialogWindow();
  }
}
