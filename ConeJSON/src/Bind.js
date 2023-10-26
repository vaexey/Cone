const util = require('./util');
const Scale = require('./Scale');
const Trigger = require('./Trigger');

exports.expandBindNode = (src) => {
    if(src === undefined)
        src = {}

    let bn = JSON.parse(JSON.stringify(src));

    if(bn.device === undefined)
        bn.ERROR = "No device specified!"

    Scale.expandScaleProperty(bn, "scale")
    Scale.expandScaleProperty(bn, "prescaler")

    bn.trigger = Trigger.expandTrigger(bn.trigger)

    return bn;
}