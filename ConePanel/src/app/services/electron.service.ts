import { IpcRendererEvent } from 'electron';
import { Injectable, NgZone } from '@angular/core';

// Electron wrapper
interface IPC {
  send(channel: string, ...data: any[]): void;

  on(channel: string, listener: (event: IpcRendererEvent, ...args: any[]) => void): void;

  once(channel: string, listener: (event: IpcRendererEvent, ...args: any[]) => void): void;

  removeAllListeners(channel: string): void;
}

declare global
{
  interface Window
  {
    ipc: IPC;
  }
}

type EventCallback = (event: IpcRendererEvent, ...args: any[]) => void;

@Injectable({
  providedIn: 'root'
})
export class ElectronService {

  private readonly _ipc: IPC | undefined = undefined;

  public maximized: boolean = false;
  public minimized: boolean = false;

  constructor(
    private zone: NgZone
  ) 
  {
    if (window.require) {
      this._ipc = window.require('electron').ipcRenderer;
      this.connectListeners();
    } else {
      console.error('Electron\'s IPC was not loaded');
    }
  }

  public on(channel: string, cb: EventCallback): void {
    if (this._ipc !== undefined) {
      this._ipc.on(channel, (event, ...args) => {
        this.zone.run(() => cb(event, ...args));
      });
    }
  }

  public once(channel: string, cb: EventCallback): void {
    if (this._ipc !== undefined) {
      this._ipc.once(channel, (event, ...args) => {
        this.zone.run(() => cb(event, ...args));
      });
    }
  }

  public send(channel: string, data?: any): void {
    if (this._ipc !== undefined) {
      return this._ipc.send(channel, data);
    }
  }

  public get ready(): boolean {
    return this._ipc !== undefined;
  }

  private connectListeners(): void
  {
    this.on("window:minimized", (event, value) => {
      this.minimized = value;
    })

    this.on("window:maximized", (event, value) => {
      if(value != this.maximized)
        this.maximized = value;
    })
  }

  public setMinimized(value: boolean): void
  {
    this.send("window:minimized", value);
    this.minimized = value;
  }

  public setMaximized(value: boolean): void
  {
    this.send("window:maximized", value);
    this.maximized = value;
  }

  public close(): void
  {
    this.send("window:close");
  }
}
