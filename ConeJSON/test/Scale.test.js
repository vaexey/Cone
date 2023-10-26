const Scale = require('./../src/Scale');

test('scale default value', () => {
    expect(Scale.expandScale()).toMatchObject({
        from: [0, 1],
        to: [0, 1],
        modulo: 0,
        round: -1
    })
})

test('scale integer value', () => {
    let i = 5;
    expect(Scale.expandScale(5)).toMatchObject({
        from: [0, 1],
        to: [0, i],
        modulo: 0,
        round: -1
    })
})

test('scale array conversion', () => {
    expect(Scale.expandScale([-1,5,2,3])).toMatchObject({
        from: [-1, 5],
        to: [2, 3],
        modulo: 0,
        round: -1
    })
});

test('scale property double', () => {
    let obj = {
        nameFrom: [5, 8],
        nameTo: [10, 20]
    }

    Scale.expandScaleProperty(obj, "name");

    expect(obj).toMatchObject({
        name: {
            from: [5,8],
            to: [10,20],
            modulo: 0,
            round: -1
        }
    })
})

test('scale property single', () => {
    let obj = {
        nameTo: [10, 20]
    }

    Scale.expandScaleProperty(obj, "name");

    expect(obj).toMatchObject({
        name: {
            from: [0,1],
            to: [10,20],
            modulo: 0,
            round: -1
        }
    })
})

test('scale unset round and modulo', () => {
    let obj = {
        a: {
            from: [5, 7],
            to: [-2, 5]
        }
    }

    Scale.expandScaleProperty(obj, "a");

    expect(obj).toMatchObject({
        a: {
            from: [5, 7],
            to: [-2, 5],
            modulo: 0,
            round: -1
        }
    })
})