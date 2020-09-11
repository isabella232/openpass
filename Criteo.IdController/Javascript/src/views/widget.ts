import IdController from '../IdController';

declare global {
    interface Window {
        __idcapi(): Promise<string | undefined>;
    }
}

window.__idcapi = IdController.getIfa;
