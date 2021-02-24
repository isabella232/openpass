// Use require to inject the code (cannot use import because it's not a TS module)
// This is a workaround so we can test that the IdController API works as expected
import IdController from '../src/IdController';

const cookieName = 'openpass_token';

describe('IdController API', () => {
    let spyCookie: jest.SpyInstance<string, any>;

    let mockedCookie: string;
    const mockIfaCookie = (value: string | undefined): void => {
        mockedCookie = value ? `${cookieName}=${value}` : '';
    }

    beforeEach(() => {
        spyCookie = jest.spyOn(document, 'cookie', 'get').mockImplementation(() => mockedCookie);
    });

    afterEach(() => {
        mockIfaCookie(undefined);
        spyCookie.mockClear();
    });

    test('returns IFA if exists', async () => {
        const ifa = 'mockedIFA';
        mockIfaCookie(ifa);
        const returnedIfa = await IdController.getIfa();
        expect(returnedIfa).toEqual(ifa);
    });

    test('returns undefined when IFA does not exist', async () => {
        const returnedIfa = await IdController.getIfa();
        expect(returnedIfa).toEqual(undefined);
    });
});
