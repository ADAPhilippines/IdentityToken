import CardanoWalletInterop from "./CardanoWalletInterop";
import ICardanoDAPPConnector from "./Interfaces/ICardanoDAPPConnector";

declare global {
    interface Window {
        CardanoWalletInterop: CardanoWalletInterop;
        cardano: ICardanoDAPPConnector;
        ScrollToElementBottom: (selector: string) => void;
        GetElementScrollTop: (selector: string) => void;
        ScrollToMessageId: (id: string) => void;
    }
}