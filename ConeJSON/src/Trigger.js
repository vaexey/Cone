const Scale = require('./Scale');

exports.expandTrigger = (src) => {
    if(src === undefined)
        src = {}

    src = JSON.parse(JSON.stringify(src));

    if(src.min === undefined)
        src.min = 0;
    
    if(src.max === undefined)
        src.max = 1;
    
    if(src.value === undefined)
        src.value = false;
    
    if(src.poll === undefined)
        src.poll = true;

    src.scale = Scale.expandScale(src.scale)

    return src
}