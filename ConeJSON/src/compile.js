const JSON5 = require('json5');
const fs = require('fs/promises');

const util = require('./util.js');

const Device = require('./Device');
const Entry = require('./Entry');

exports.compile = async (srcPath, destPath) => {
    let json = (await fs.readFile(srcPath)).toString();

    let i = json.indexOf("$include:");
    while(i != -1)
    {
        var word = json.substring(i + 1)
        word = word.substring(0, word.indexOf("$"))

        var path = word.substring("include:".length);

        let inc = (await fs.readFile(`./config/` + path)).toString();

        json = json.replaceAll(`$${word}$`, inc)
        i = json.indexOf("$include:");
    }

    const src = JSON5.parse(json);

    const dest = exports.compileRaw(src);
    const write = JSON.stringify(dest);
    // const write = JSON.stringify(dest, null, 4);

    await fs.writeFile(destPath, write);
}

exports.compileRaw = (src) => {
    let macros = src.macros;

    let devices = src.devices.map(Device.expandDevice);
    let entries = util.applyMacros(
        macros,
        src.entries
    ).map(Entry.expandEntry);

    return {
        devices,
        entries,
    }
}



