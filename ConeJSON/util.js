/**
 * Converts ID literal to friendly name
 * @param {string} id 
 * @returns string
 */
exports.idToName = (id) => {
    const words = id.replace("-", "_").split(" ");

    return words
        .map(w => w.substring(0, 1).toUpperCase() + w.substring(1))
        .filter(w => w.trim().length > 0)
        .join(" ");
}

/**
 * Converts friendly name to ID literal
 * @param {string} name 
 * @returns string
 */
exports.nameToId = (name) => {
    const arr = name.toLowerCase().split("").map(ch => ch.charCodeAt(0))

    return String.fromCharCode
        .apply(null, 
            arr.filter(ch => ch >= 33 && ch <= 126)
        )
}

/**
 * Makes sure that specified key in object is an array
 * @param {Map<string, any>} object 
 * @param {string} key 
 */
exports.arrayKey = (object, key) => {
    if(!object[key])
    {
        object[key] = []
        return;
    }

    if(!Array.isArray(object[key]))
    {
        if(Object.keys(object[key]).length == 0)
        {
            object[key] = []
            return;
        }

        object[key] = [object[key]]
    }
}

/**
 * If target key does not exist try to use shortcut instead.
 * If both not possible, use default value.
 * @param {any} object 
 * @param {string} target 
 * @param {string} shortcut 
 * @param {any} def 
 */
exports.keyShortcuts = (object, target, shortcut, def) => {
    if(object[target] === undefined)
    {
        if(object[shortcut] !== undefined)
        {
            object[target] = object[shortcut]
        }
        else
        {
            object[target] = def
        }
    }

    if(object[shortcut] !== undefined)
        delete object[shortcut]
}