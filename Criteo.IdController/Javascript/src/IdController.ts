// IdControllerWidget class must be initialized from the front-end
//  Examples:
//      const widget = new IdControllerWidget('modal');
//      const widget = new IdControllerWidget('custom', 'name-of-div');

interface Options {
    parentId: string;
    height: string;
    width: string;
}

export default class IdController {
    private readonly widgetUrl = '__MACRO-CONTROLLER-URI__/api/widget';
    // __MACRO-CONTROLLER-URI__ will be replaced by correct URI depending on build config
    private readonly positions: Array<string> = ["modal", "custom"];

    private readonly position!: string;
    private readonly parent!: HTMLElement;
    private readonly height!: string;
    private readonly width!: string;
    private readonly originHost!: string;
    private iframe!: HTMLElement;

    private readonly expiryHours = 13 * 24 * 30;
    public static readonly ifaFirstPartyCookieName = "id_controller_ifa";

    constructor(position: string, userOptions: Partial<Options> = {}) {
        // Fill options with default ones
        const options: Options = Object.assign({
            parentId: "",
            height: "100%",
            width: "100%",
        }, userOptions);

        // Compute attributes
        if (this.positions.indexOf(position) === -1) {
            console.error("[IdControllerWidget] The provided position to display the widget is not valid.");
            return;
        }
        this.position = position;

        const clientParent = (this.position === "modal")
            ? document.body
            : document.getElementById(options.parentId);

        if (!clientParent) {
            console.error("[IdControllerWidget] Parent not found.");
            return;
        }

        // Always create a new child div to keep a shared structure (client div -> parent -> iframe)
        // Parent is the div we just created (may be removed at the end if needed)
        const parent = document.createElement("div");
        clientParent.appendChild(parent);
        this.parent = parent;
        this.height = options.height;
        this.width = options.width;

        this.originHost = this.getCurrentHostname();

        // Display widget
        this.addStyleToParent();
        this.createAndAppendIframe();
        this.addSetCookieEventListener();
    }

    private getCurrentHostname(): string {
        return window.location.hostname;
    }

    private addStyleToParent(): void {
        if (this.position === "modal") {
            this.parent.style.background = "rgba(0, 0, 0, 0.5)";
            this.parent.style.height = "100vh";
            this.parent.style.left = "0";
            this.parent.style.position = "absolute";
            this.parent.style.top = "0";
            this.parent.style.width = "100vw";
        } else {
            this.parent.style.height = "100%";
            this.parent.style.width = "100%";
        }
    }

    private addStyleToIframe(iframe: HTMLIFrameElement): void {
        iframe.style.border = "none";

        if (this.position === "modal") {
            iframe.style.left = "50%";
            iframe.style.top = "50%";
            iframe.style.transform = "translate(-50%, -50%)";
            iframe.style.position = "fixed";
            iframe.style.display = "block";
            iframe.style.zIndex = "2147483647";
            iframe.height = "200px"; // TODO: Make this customizable
            iframe.width = "400px"; // TODO: Make this customizable
        } else {
            iframe.height = this.height;
            iframe.width = this.width;
        }
    }

    private createAndAppendIframe(): void {
        const iframe = document.createElement("iframe");
        iframe.src = `${this.widgetUrl}?originHost=${this.originHost}`;
        this.addStyleToIframe(iframe);
        this.iframe = iframe;
        this.parent.appendChild(this.iframe);

        if (this.position === "modal")
            this.addClickListenerToCloseModal();
    }

    private removeWidget(): void {
        // Remove created parent div and iframe (leave the client provided div intact)
        this.parent.remove();
    }

    private addClickListenerToCloseModal(): void {
        const closeModalWrapper = (event: Event) => {
            this.removeWidget();
            document.removeEventListener('click', closeModalWrapper); // remove itself
        };

        document.addEventListener('click', closeModalWrapper);
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
                IdController.setCookieString(IdController.ifaFirstPartyCookieName,
                    data.ifa,
                    expires,
                    this.originHost,
                    document);
            }

            // Remove modal
            if (this.position === "modal")
                this.removeWidget();
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
