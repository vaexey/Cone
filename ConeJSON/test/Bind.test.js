const Bind = require('./../src/Bind');

test('bind node has required properties', () => {
    let ent = Bind.expandBindNode({
        device: "abc"
    })

    expect(ent).toHaveProperty("scale")
    expect(ent).toHaveProperty("prescaler")
    expect(ent).toHaveProperty("trigger")
})