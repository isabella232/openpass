// IdControllerWidget class must be initialized from the front-end
// example: const widget = new IdControllerWidget();

class IdControllerWidget {
    private readonly widgetUrl = 'http://localhost:1234/api/widget'; // TODO: Change URL when deployed

    private readonly parent!: HTMLElement;
    private readonly height!: number;
    private readonly width!: number;

    constructor(parentId: string, height: number = 250, width: number = 500) {
        const parent = document.getElementById(parentId);
        if (!parent) {
            console.error("[IdControllerWidget] Parent not found.");
            return;
        }

        this.parent = parent;
        this.height = height;
        this.width = width;

        this.createAndAppendIframe();
    }

    private createAndAppendIframe() {
        const iframe = document.createElement("iframe");
        iframe.src = this.widgetUrl;
        iframe.height = this.height.toString();
        iframe.width = this.width.toString();
        iframe.style.border = "none";

        this.parent.appendChild(iframe);
    }
}
