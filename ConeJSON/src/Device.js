const util = require('./util');

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
