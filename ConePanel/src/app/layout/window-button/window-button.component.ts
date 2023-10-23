import { Component, EventEmitter, Input, Output } from '@angular/core';

export type WindowButtonType = 'close' | 'minimize' | 'maximize';

@Component({
  selector: 'app-window-button',
  templateUrl: './window-button.component.html',
  styleUrls: ['./window-button.component.scss']
})
export class WindowButtonComponent
{
  @Input() type: WindowButtonType = "close";
  @Input() state: boolean = false;
  @Output() bclick = new EventEmitter();
}
