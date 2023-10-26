const Entry = require('./../src/Entry');

test('entry id generation', () => {
    let e1 = Entry.expandEntry({});
    let e2 = Entry.expandEntry({});
    let e3 = Entry.expandEntry({});

    expect(e1).not.toBe(e2)
    expect(e2).not.toBe(e3)
    expect(e3).not.toBe(e1)
})

test('entry has name and id', () => {
    let ent = Entry.expandEntry({})

    expect(ent).toHaveProperty("id")
    expect(ent).toHaveProperty("name")
})

test('bind entry has required properties', () => {
    let ent = Entry.expandEntry({
        type: "bind"
    })

    expect(Array.isArray(ent.input)).toBe(true)
    expect(Array.isArray(ent.output)).toBe(true)

    expect(['both', 'forward']).toContain(ent.direction)

    expect(ent).toHaveProperty("iscale")
    expect(ent).toHaveProperty("oscale")
})

test('event entry has required properties', () => {
    let ent = Entry.expandEntry({
        type: "event"
    })

    expect(Array.isArray(ent.trigger)).toBe(true)
    expect(Array.isArray(ent.actions)).toBe(true)
})