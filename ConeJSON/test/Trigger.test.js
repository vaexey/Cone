const Trigger = require('./../src/Trigger');

test('trigger default value', () => {
    expect(Trigger.expandTrigger({})).toMatchObject({
        min: 0,
        max: 1,
        value: false,
        poll: true,
        change: false,
        scale: {
            from: [0,1],
            to: [0, 1]
        }
    })
})