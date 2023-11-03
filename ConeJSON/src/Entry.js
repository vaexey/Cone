const util = require('./util');
const Bind = require('./Bind');
const Scale = require('./Scale');
const Event = require('./Event');
const Trigger = require('./Trigger');

let entCounter = 0;
exports.expandEntry = (src) => {
    let ent = src

    if(typeof src === 'object')
        ent = JSON.parse(JSON.stringify(ent));

    if(ent.id === undefined && ent.name === undefined)
    {
        ent.id = `uent${++entCounter}`;
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
    
        ent.input = ent.input.map(Bind.expandBindNode)
        ent.output = ent.output.map(Bind.expandBindNode)
        
        ent.trigger = Trigger.expandTrigger(ent.trigger);

        Scale.expandScaleProperty(ent, "iscale")
        Scale.expandScaleProperty(ent, "oscale")
    }

    if(ent.type.toLowerCase() == "event")
    {
        ent.type = "event";

        util.arrayKey(ent, "trigger");
        util.arrayKey(ent, "actions");

        ent.trigger = ent.trigger.map(Event.expandEventNode)
        ent.actions = ent.actions.map(Event.expandEventNode)

        if(ent.basic === undefined)
            ent.basic = true
    }

    return ent;
}