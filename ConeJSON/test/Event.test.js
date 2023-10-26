const Event = require('./../src/Event');

test('event node creates bind node', () => {
    expect(Event.expandEventNode({
        bind: {}
    }).bind).toHaveProperty("scale")
})