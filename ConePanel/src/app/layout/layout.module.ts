import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WindowComponent } from './window/window.component';
import { WindowButtonsComponent } from './window-buttons/window-buttons.component';
import { WindowButtonComponent } from './window-button/window-button.component';



@NgModule({
  declarations: [
    WindowComponent,
    WindowButtonsComponent,
    WindowButtonComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    WindowComponent
  ]
})
export class LayoutModule { }
