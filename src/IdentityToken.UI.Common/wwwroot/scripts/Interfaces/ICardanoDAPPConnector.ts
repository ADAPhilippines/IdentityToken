import Paginate from "../Types/Paginate";

interface ICardanoDAPPConnector {
    enable(): Promise<boolean>;

    isEnabled(): Promise<boolean>;

    getUsedAddresses(paginate?: Paginate): Promise<string[]>;

    getUtxos(): Promise<string[]>;

    signTx(tx: string): Promise<string>;
}

export default ICardanoDAPPConnector;