import CardanoWalletInterop from "./CardanoWalletInterop";
import ICardanoDAPPConnector from "./Interfaces/ICardanoDAPPConnector";
import IDotNetObjectRef from "./Interfaces/IDotNetObjectRef";

declare global {
    interface Window {
        CardanoWalletInterop: CardanoWalletInterop;
        cardano: ICardanoDAPPConnector;
        ScrollToElementBottom: (selector: string) => void;
        GetElementScrollTop: (selector: string) => void;
        ScrollToMessageId: (id: string) => void;
        GenerateQRDataUrlAsync: (data: string) => Promise<string>;
        CopyToClipboardAsync: (data: string) => Promise<void>;
        AttachEmojiHandler: (objRef: IDotNetObjectRef, handlerName: string) => void;
    }
}