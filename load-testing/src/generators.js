import * as faker from 'faker/locale/en_US';

export const getRequest = () => ({
    "smart_watch_slug": faker.random.alphaNumeric(10),
    "start": faker.date.recent(1),
    "end": faker.date.soon(30)
});

export const postRequest = () => ({
    "smart_watch_slug": faker.random.alphaNumeric(10),
    "heart_rate_monitor": generateObjectsBetween(1, 2, generateHeartRateMonitor)
});

const generateHeartRateMonitor = () => ({
    "date": faker.date.recent(1),
    "avgValues": generateObjectsBetween(1, 48, generateHeartRateValue)
});

const generateHeartRateValue = () => ({
    "registered_at": faker.date.recent(1),
    "heart_rate": faker.datatype.number({ min: 60, max: 130 }),
    "max": faker.datatype.number({ min: 60, max: 130 }),
    "min": faker.datatype.number({ min: 60, max: 130 })
});

const generateObjectsBetween = (min, max, generator) => {
    let objects = [];
    for (let i = 0; i < faker.datatype.number({ min: min, max: max }); i++) {
        objects.push(generator());
    }
    return objects;
}
