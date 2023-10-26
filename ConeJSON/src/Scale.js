exports.expandScaleProperty = (bn, prop) => {
    const scaleTo = prop + "To"
    const scaleFrom = prop + "From"

    if(bn[prop] === undefined
        && (bn[scaleFrom] !== undefined || bn[scaleTo] !== undefined))
    {
        bn[prop] = [0,1,0,1];

        if(bn[scaleFrom] !== undefined)
        {
            if(!Array.isArray(bn[scaleFrom]))
                bn[scaleFrom] = [0, bn[scaleFrom]]

            bn[prop][0] = bn[scaleFrom][0]
            bn[prop][1] = bn[scaleFrom][1]

            delete bn[scaleFrom];
        }

        if(bn[scaleTo] !== undefined)
        {
            if(!Array.isArray(bn[scaleTo]))
                bn[scaleTo] = [0, bn[scaleTo]]

            bn[prop][2] = bn[scaleTo][0]
            bn[prop][3] = bn[scaleTo][1]

            delete bn[scaleTo];
        }
    }

    bn[prop] = this.expandScale(bn[prop])
}

exports.expandScale = (src) => {
    if(src === undefined)
        src = 1;

    scale = JSON.parse(JSON.stringify(src))
    
    if(typeof scale === 'number')
    {
        scale = [0, 1, 0, scale]
    }

    if(Array.isArray(scale))
    {
        scale = {
            from: [scale[0], scale[1]],
            to: [scale[2], scale[3]],
            modulo: 0,
            round: -1
        }
    }

    if(scale.modulo === undefined)
        scale.modulo = 0;
    
    if(scale.round === undefined)
        scale.round = -1;

    scale.from = scale.from.map(v => +v)
    scale.to = scale.to.map(v => +v)

    return scale
}