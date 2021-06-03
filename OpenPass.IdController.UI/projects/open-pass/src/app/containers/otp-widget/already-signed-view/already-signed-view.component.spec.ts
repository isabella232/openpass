import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AlreadySignedViewComponent } from './already-signed-view.component';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxsModule } from '@ngxs/store';
import { TranslateModule } from '@ngx-translate/core';
import { WINDOW } from '@utils/injection-tokens';
import { Component } from '@angular/core';

const windowFactory = () => ({
  opener: {
    postMessage: () => {},
  },
  postMessage: () => {},
});

@Component({
  selector: 'usrf-copyright',
  template: '',
})
class StubCopyrightComponent {}

describe('AlreadySignedViewComponent', () => {
  let component: AlreadySignedViewComponent;
  let fixture: ComponentFixture<AlreadySignedViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AlreadySignedViewComponent, StubCopyrightComponent],
      imports: [RouterTestingModule, NgxsModule.forRoot([]), TranslateModule.forRoot()],
      providers: [{ provide: WINDOW, useFactory: windowFactory }],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AlreadySignedViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
