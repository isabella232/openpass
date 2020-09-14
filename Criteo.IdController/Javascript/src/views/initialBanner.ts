import {sendEvent, addPostMessageEvtListener, EventType} from "../utils";

function openDialog() {
    window.open(window.idController?.optionsUrl || "", "_blank", "width=400,height=520,top=200,left=800,location=no");
}

document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("openOptionsBanner")?.addEventListener("click", openDialog);
    sendEvent(EventType.BannerRequest, window.idController?.originHost);
    addPostMessageEvtListener(window);
})