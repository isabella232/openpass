// IdControllerWidget class must be initialized from the front-end
// example: const widget = new IdControllerWidget();

class IdControllerWidget {
    private readonly widgetUrl = 'https://id-controller.crto.in/api/widget';

    private readonly parent!: HTMLElement;
    private readonly height!: number;
    private readonly width!: number;
    private readonly originHost!: string;

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
}
