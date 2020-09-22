import { sendEvent, fetchIfa, EventType } from "../utils";

interface FormElements extends HTMLFormControlsCollection {
    choice: Array<HTMLInputElement>,
}

document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById('options') as HTMLFormElement;

    form?.addEventListener('submit',
        async (event) => {
            event.preventDefault();

            const elements = form.elements as FormElements;
            const checkedOption = Array.from(elements.choice).find((radio) => radio.checked)
            const eventType = (checkedOption?.value === "true")
                ? EventType.ConsentGranted
                : EventType.ConsentNotGranted;

            const hostname = window.idController.originHost;
            sendEvent(eventType, hostname);

            let ifa : string | undefined = undefined;
            if (eventType == EventType.ConsentGranted)
                ifa = await fetchIfa();

            window.opener.postMessage({
                isIdControllerMessage: true,
                ifa: ifa
            }, "*");

            window.close();
        }
    );
})