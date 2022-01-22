import Paginate from "./Types/Paginate";

export type BifrostWalletMetadata = {
    apiVersion: string;
    icon: string;
    name: string;
    id: string;
}

export type BifrostPaginate = {
    page: number,
    limit: number,
};

export type BifrostWalletApi = {
    getUsedAddresses(paginate?: BifrostPaginate): Promise<string[]>;
    getUtxos(): Promise<string[]>;
    signTx(tx: string): Promise<string>;
}

export type BifrostWalletId = string | "nami" | "ccvault" | "flint";

export class Bifrost {
    static _cardano = window.cardano as any;
    static _api?: BifrostWalletApi = undefined;

    public static getWallets(): BifrostWalletMetadata[] {
        const result: BifrostWalletMetadata[] = [];
        for (const i in Bifrost._cardano) {
            const cardano = Bifrost._cardano;
            const p = cardano[i];
            if (p.apiVersion !== null && p.icon != null && p.name !== null) {
                result.push({
                    apiVersion: p.apiVersion,
                    icon: p.icon,
                    name: p.name,
                    id: i.toString()
                });
            }
        }
        return result;
    }

    public static async enableAsync(id: BifrostWalletId): Promise<boolean> {
        try {
            const result = id == "nami" ? await Bifrost._cardano.enable() : await Bifrost._cardano[id].enable();
            if (typeof result === "boolean") Bifrost._api = Bifrost._cardano;
            else Bifrost._api = result;
            return true;
        } catch (ex) {
            console.log(ex);
            return false;
        }
    }

    public static async isEnabledAsync(id: BifrostWalletId) {
        return await id === "nami" ? Bifrost._cardano.isEnabled() : Bifrost._cardano[id].isEnabled();
    }

    public static async setWalletAsync(id: BifrostWalletId) {
        if (await Bifrost.isEnabledAsync(id)) {
            await Bifrost.enableAsync(id);
        } else throw "Wallet is not enabled.";
    }

    public static async signTxRawAsync(txCborHex: string): Promise<string> {
        if (Bifrost._api !== undefined)
            return await Bifrost._api.signTx(txCborHex);    
        else
            throw "No API available, is the wallet connection enabled?";
    }

    public static async getUsedAddressesRawAsync(): Promise<string[]> {
        if (Bifrost._api !== undefined)
            return await Bifrost._api.getUsedAddresses();
        else
            throw "No API available, is the wallet connection enabled?";
    }

    public static async getUtxosRawAsync(): Promise<string[]> {
        if (Bifrost._api !== undefined)
            return await Bifrost._api.getUtxos();
        else
            throw "No API available, is the wallet connection enabled?";
    }
}
