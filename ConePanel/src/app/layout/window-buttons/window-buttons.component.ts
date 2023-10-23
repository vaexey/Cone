import { Component } from '@angular/core';
import { ElectronService } from 'src/app/services/electron.service';

@Component({
  selector: 'app-window-buttons',
  templateUrl: './window-buttons.component.html',
  styleUrls: ['./window-buttons.component.scss']
})
export class WindowButtonsComponent {

  constructor(
    public electron: ElectronService
  ) 
  {}

  public minimize(): void
  {
    this.electron.setMinimized(true)
  }

  public maximize(): void
  {
    this.electron.setMaximized(!this.electron.maximized);
  }

  public close(): void
  {
    this.electron.close();
  }
}
