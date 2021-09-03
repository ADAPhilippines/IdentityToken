import CardanoWalletInterop from "./CardanoWalletInterop";
import ICardanoDAPPConnector from "./Interfaces/ICardanoDAPPConnector";

declare global {
    interface Window {
        CardanoWalletInterop: CardanoWalletInterop;
        cardano: ICardanoDAPPConnector;
    }
}