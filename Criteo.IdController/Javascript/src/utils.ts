// Handy functions used by static sites

const eventUrl = '__MACRO-CONTROLLER-URI__/api/event';
const getIfaUrl = '__MACRO-CONTROLLER-URI__/api/ifa';
// __MACRO-CONTROLLER-URI__ will be replaced by the correct URI depending on build config

export enum EventType {
    Unknown = 0,
    BannerRequest = 1,
    ConsentGranted = 2,
    ConsentNotGranted = 3,
    BannerIgnored = 4,
    LearnMore = 5,
    EmailShared = 6
}

export function sendEvent(eventType: number, originHost: string = ""): void {
    const eventData = getEventData(eventType, originHost);
    // TODO: Get localwebid, uid or ifa from cookies (or tags)

    fetch(eventUrl, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(eventData)
    })
    .catch((error) => console.error(`[IdController] Error when sending event: ${error}`));
}

export function getEventData(
    eventType: number,
    originHost: string,
    localwebid?: string,
    uid?: string,
    ifa?: string): EventRequest {
    return {
        EventType: eventType,
        OriginHost: originHost,
        LocalWebId: localwebid,
        Uid: uid,
        Ifa: ifa
    };
}

export function addPostMessageEvtListener(currentWindow: Window) {
    const evtListener = (event: MessageEvent) => {
        const data = event.data;

        // Discard messages coming from other iframes
        if (!data || !data.isIdControllerMessage) {
            return;
        }

        // Prevent the message to propagate to other listeners
        event.stopImmediatePropagation();

        currentWindow.parent.postMessage({
            isIdControllerMessage: true,
            ifa: data.ifa,
        }, "*");
    }

    currentWindow.addEventListener("message", evtListener, true);
}

export async function fetchIfa(): Promise<string | undefined> {
    try {
        const fetchResponse = await fetch(getIfaUrl);
        if (fetchResponse.ok) {
            const response = await fetchResponse.json();
            return response ? response.ifa : undefined;
        }
        throw new Error(fetchResponse.statusText);

    } catch (error) {
        console.error(`[IdController] Error when fetching Ifa through ${getIfaUrl} : ${error}`);
        return undefined;
    }
}


