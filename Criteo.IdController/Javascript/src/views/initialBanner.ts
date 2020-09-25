import {sendEvent, addPostMessageEvtListener, EventType} from "../utils";

const openDialog = () => {
    // Options banner required sizes
    const width = 400;
    const height = 460;
    // Computed coordinates to place the dialog
    const left = (screen.width - width) / 2; // middle
    const top = (screen.height - height) / 3; // slightly below the middle (avoid bar)

    window.open(window.idController.optionsUrl || "", "_blank", `width=${width},height=${height},top=${top},left=${left},location=no`);
}

document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("openOptionsBanner")?.addEventListener("click", openDialog);
    sendEvent(EventType.BannerRequest, window.idController.originHost);
    addPostMessageEvtListener(window);
})