const Bind = require('./Bind');

exports.expandEventNode = (src) => {
    if(src === undefined)
        src = {}

    let en = JSON.parse(JSON.stringify(src))

    if(en.bind !== undefined)
    {
        en.bind = Bind.expandBindNode(en.bind)
    }

    return en
}