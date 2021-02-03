interface Window {
    idController: {
        optionsUrl?: string,
        originHost?: string,
    },
}

interface EventRequest {
    EventType: number,
    OriginHost: string,
    LocalWebId?: string,
    Uid?: string,
    Ifa?: string
}