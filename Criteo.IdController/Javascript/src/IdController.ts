// IdControllerWidget class must be initialized from the front-end
// example: const widget = new IdControllerWidget('name-of-div');

export default class IdController {
    private readonly widgetUrl = 'https://id-controller.crto.in/api/widget';

    //Use this URL for local testing
    //private readonly widgetUrl = 'http://localhost:1234/api/widget';

    private readonly parent!: HTMLElement;
    private readonly height!: number;
    private readonly width!: number;
    private readonly originHost!: string;
    private readonly expiryHours = 13 * 24 * 30;
    public static readonly ifaFirstPartyCookieName = "id_controller_ifa";

    constructor(parentId: string, height: number = 250, width: number = 500) {
        const parent = document.getElementById(parentId);
        if (!parent) {
            console.error("[IdControllerWidget] Parent not found.");
            return;
        }

        this.parent = parent;
        this.height = height;
        this.width = width;
        this.originHost = this.getCurrentHostname();

        this.createAndAppendIframe();
        this.addSetCookieEventListener();
    }

    private getCurrentHostname(): string {
        return window.location.hostname;
    }

    private createAndAppendIframe(): void {
        const iframe = document.createElement("iframe");
        iframe.src = `${this.widgetUrl}?originHost=${this.originHost}`;
        iframe.height = this.height.toString();
        iframe.width = this.width.toString();
        iframe.style.border = "none";

        this.parent.appendChild(iframe);
    }

    private static setCookieString(
        key: string,
        value: string,
        expires: string,
        domain: string | undefined,
        doc: Document,
    ) {
        let cookie = key + "=" + encodeURIComponent(value) + ";" + expires + ";";
        if (domain && domain !== "") {
            cookie += "domain=." + domain + ";";
        }
        doc.cookie = cookie + "path=/";
    }

    private addSetCookieEventListener(): void {

        const evtListener = (event: MessageEvent) => {
            const data = event.data;

            // Discard messages coming from other iframes
            if (!data || !data.isIdControllerMessage) {
                return;
            }

            // Prevent the message to propagate to other listeners
            event.stopImmediatePropagation();

            if (data.ifa) {
                const date = new Date();
                date.setTime(date.getTime() + (this.expiryHours * 60 * 60 * 1000));
                const expires = "expires=" + date.toUTCString();
                IdController.setCookieString(IdController.ifaFirstPartyCookieName, data.ifa, expires, this.originHost, document);
            }
        }

        window.addEventListener("message", evtListener, true);
    }

    public static getIfa(): Promise<string | undefined> {
        return new Promise<string | undefined>((resolve: Function, reject: Function) => {
            const cookieName = IdController.ifaFirstPartyCookieName;
            const cookie = document.cookie.match(`(^|;)\s*${cookieName}\s*=\s*([^;]+)`);
            resolve(cookie ? cookie.pop() : undefined);
        });
    }
}
