const JSON5 = require('json5');
const fs = require('fs/promises');

const util = require('./util.js');

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
    const write = JSON.stringify(dest, null, 4);

    await fs.writeFile(destPath, write);
}

exports.compileRaw = (src) => {
    let macros = src.macros;

    let devices = src.devices.map(exports.expandDevice);
    let entries = util.applyMacros(macros,src.entries).map(exports.expandEntry);

    return {
        devices,
        entries,
    }
}

let devCounter = 0;
exports.expandDevice = (src) => {
    let dev = JSON.parse(JSON.stringify(src));

    if(dev.id === undefined && dev.name === undefined)
    {
        dev.id = `udev${++devCounter}`;
    }

    if(dev.name === undefined)
    {
        dev.name = util.idToName(dev.id);
    }

    if(dev.id === undefined)
    {
        dev.id = util.nameToId(dev.name);
    }

    if(dev.plugin === undefined)
    {
        dev.ERROR = "No plugin specified!"
    }

    return dev;
}

let entCounter = 0;
exports.expandEntry = (src) => {
    let ent = JSON.parse(JSON.stringify(src));

    if(ent.id === undefined && ent.name === undefined)
    {
        ent.id = `udev${++entCounter}`;
    }

    if(ent.name === undefined)
    {
        ent.name = util.idToName(ent.id);
    }

    if(ent.id === undefined)
    {
        ent.id = util.nameToId(ent.name);
    }

    if(ent.type === undefined)
    {
        ent.ERROR = "No type specified!"
        return ent;
    }

    if(ent.type.toLowerCase() == "bind")
    {
        ent.type = "bind";
        
        util.arrayKey(ent, "input");
        util.arrayKey(ent, "output");
    
        util.keyShortcuts(ent, "direction", "dir", "forward");
    
        ent.input = ent.input.map(exports.expandBindNode)
        ent.output = ent.output.map(exports.expandBindNode)
    }

    if(ent.type.toLowerCase() == "event")
    {
        ent.type = "event";

        util.arrayKey(ent, "trigger");
    }

    return ent;
}

exports.expandBindNode = (src) => {
    let bn = JSON.parse(JSON.stringify(src));

    if(bn.device === undefined)
        bn.ERROR = "No device specified!"

    if(bn.scale === undefined
        && (bn.scaleFrom !== undefined || bn.scaleTo !== undefined))
    {
        bn.scale = [0,1,0,1];

        if(bn.scaleFrom !== undefined)
        {
            if(!Array.isArray(bn.scaleFrom))
                bn.scaleFrom = [0, bn.scaleFrom]

            bn.scale[0] = bn.scaleFrom[0]
            bn.scale[1] = bn.scaleFrom[1]

            delete bn.scaleFrom;
        }

        if(bn.scaleTo !== undefined)
        {
            if(!Array.isArray(bn.scaleTo))
                bn.scaleTo = [0, bn.scaleTo]

            bn.scale[2] = bn.scaleTo[0]
            bn.scale[3] = bn.scaleTo[1]

            delete bn.scaleTo;
        }
    }

    if(bn.scale === undefined)
        bn.scale = 1;
    
    if(typeof bn.scale === 'number')
    {
        bn.scale = [0, 1, 0, bn.scale]
    }

    if(Array.isArray(bn.scale))
    {
        bn.scale = {
            from: [bn.scale[0], bn.scale[1]],
            to: [bn.scale[2], bn.scale[3]]
        }
    }

    bn.scale.from = bn.scale.from.map(v => +v)
    bn.scale.to = bn.scale.to.map(v => +v)

    return bn;
}