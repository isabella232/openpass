import { sendEvent, EventType } from "../utils";

function goBackToPreviousSite() {
    window.history.back();
}

document.addEventListener("DOMContentLoaded" ,() => {
    document.getElementById("goBackToPreviousSite")?.addEventListener("click", goBackToPreviousSite);
    sendEvent(EventType.LearnMore,  window.idController.originHost);
})