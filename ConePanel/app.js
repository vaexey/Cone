const {app, BrowserWindow, ipcMain} = require('electron')
const url = require("url");
const path = require("path");

/**
 * @type {BrowserWindow}
 */
let mainWindow

function createWindow () {
    mainWindow = new BrowserWindow({
        width: 800,
        height: 600,
        transparent: true,
        frame: false,
        icon: "./src/assets/cone.png",
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false
        }
    })

    // const url = url.format({
    //     pathname: path.join(__dirname, `/dist/electron-app/index.html`),
    //     protocol: "file:",
    //     slashes: true
    // })
    const url = "http://localhost:4200"

    mainWindow.loadURL(
        url
    );

    // Open the DevTools.
    // mainWindow.webContents.openDevTools()

    mainWindow.on('closed', function () {
        mainWindow = null
    })

    ipcMain.on('window:maximized', (event, value) => {
        if(value === undefined)
            return;

        if(value)
        {
            mainWindow.maximize();
        }else{
            mainWindow.unmaximize();
        }
    })

    ipcMain.on('window:minimized', (event, value) => {
        if(value === undefined)
            return;

        if(value)
        {
            mainWindow.minimize();
        }else{
            mainWindow.restore();
        }
    })

    ipcMain.on('window:close', () => {
        mainWindow.close();
    })

    mainWindow.on('maximize', () => {
        ipcMain.emit("window:maximized", true);
    })
    mainWindow.on('unmaximize', () => {
        ipcMain.emit("window:maximized", false);
    })

    mainWindow.on('minimize', () => {
        ipcMain.emit("window:minimized", true);
    })
    mainWindow.on('restore', () => {
        ipcMain.emit("window:minimized", false);
    })
}

app.on('ready', createWindow)