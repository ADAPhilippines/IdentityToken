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

export type BifrostWalletId = string | "nami" | "ccvault" | "flintExperimental";

const TEMP_WALLETS: BifrostWalletMetadata[] = [
    {
        apiVersion: "unknown",
        name: "Nami Wallet",
        icon: "data:image/svg+xml;base64,PHN2ZyBpZD0iTGF5ZXJfMSIgZGF0YS1uYW1lPSJMYXllciAxIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB3aWR0aD0iMjkuMTI3IiBoZWlnaHQ9IjI5Ljk0NyIgdmlld0JveD0iMCAwIDI5LjEyNyAyOS45NDciPgogIDxkZWZzPgogICAgPGxpbmVhckdyYWRpZW50IGlkPSJsaW5lYXItZ3JhZGllbnQiIHkxPSIwLjUiIHgyPSIxIiB5Mj0iMC41IiBncmFkaWVudFVuaXRzPSJvYmplY3RCb3VuZGluZ0JveCI+CiAgICAgIDxzdG9wIG9mZnNldD0iMC4yMSIgc3RvcC1jb2xvcj0iIzJhYjdhZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjAuOCIgc3RvcC1jb2xvcj0iI2RmNmMwMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50LTIiIHgxPSIwLjAxMSIgeTE9Ii0wLjA4OCIgeDI9IjUuNTMzIiB5Mj0iNi41NTciIHhsaW5rOmhyZWY9IiNsaW5lYXItZ3JhZGllbnQiLz4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50LTMiIHgxPSItMi4zMjEiIHkxPSItMi44NDYiIHgyPSIzLjI2OSIgeTI9IjMuOCIgeGxpbms6aHJlZj0iI2xpbmVhci1ncmFkaWVudCIvPgogICAgPGxpbmVhckdyYWRpZW50IGlkPSJsaW5lYXItZ3JhZGllbnQtNCIgeDE9Ii0yLjkxNSIgeTE9Ii0zLjYwOSIgeDI9IjIuNjA2IiB5Mj0iMy4wMzQiIHhsaW5rOmhyZWY9IiNsaW5lYXItZ3JhZGllbnQiLz4KICA8L2RlZnM+CiAgPHBhdGggaWQ9IlBmYWRfNzM0IiBkYXRhLW5hbWU9IlBmYWQgNzM0IiBkPSJNMTE4LjIyNSw4Ljc0MmwtLjctLjcyM2ExLjQzNCwxLjQzNCwwLDAsMS0xLjIxLDEuMzA2bC43MjIuNjcyLDI1Ljg5NCwyNC4xNFptMS43MDksN1YzMC4xNDlhMS40MjQsMS40MjQsMCwwLDEsMS45MjUtLjEyOFYyMC4zNTJsMTUuNiwxNS4yMTNoMi42ODlabTE4LjE3NS0zLjMyOWExLjQzMSwxLjQzMSwwLDAsMS0xLS40djkuNzUzTDEyMS44LDYuNTVoLTIuOTY2bDIwLjIxLDE5Ljc4NFYxMi4wNjRBMS40MjQsMS40MjQsMCwwLDEsMTM4LjEwOSwxMi40MDhaIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtMTEzLjc5OSAtNS42MTcpIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudCkiLz4KICA8cGF0aCBpZD0iUGZhZF83MzUiIGRhdGEtbmFtZT0iUGZhZCA3MzUiIGQ9Ik0xMDAuOTczLDBhMi4yOTEsMi4yOTEsMCwxLDAsMi4yOSwyLjI5MUEyLjI5MSwyLjI5MSwwLDAsMCwxMDAuOTczLDBabS4yMTgsMy43YTEuMzc3LDEuMzc3LDAsMCwxLS4yMTguMDE3LDEuNDM0LDEuNDM0LDAsMSwxLDEuNDMzLTEuNDMzVjIuNEExLjQzNCwxLjQzNCwwLDAsMSwxMDEuMTkxLDMuN1oiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC05OC42OCAwKSIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQtMikiLz4KICA8cGF0aCBpZD0iUGZhZF83MzYiIGRhdGEtbmFtZT0iUGZhZCA3MzYiIGQ9Ik0yNTUuOTM1LDIxLjU0YTIuMjkxLDIuMjkxLDAsMCwwLDAsNC41ODEsMi4yNTQsMi4yNTQsMCwwLDAsLjkzLS4yLDIuMjksMi4yOSwwLDAsMC0uOTMtNC4zODNabS45MywzLjM3OWExLjQzMywxLjQzMywwLDEsMSwuNS0xLjA4OUExLjQzMywxLjQzMywwLDAsMSwyNTYuODY1LDI0LjkxOVoiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0yMzEuNjI2IC0xOC40NzMpIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudC0zKSIvPgogIDxwYXRoIGlkPSJQZmFkXzczNyIgZGF0YS1uYW1lPSJQZmFkIDczNyIgZD0iTTEzNi4xMjQsMTYzLjMyOGEyLjI5MSwyLjI5MSwwLDEsMCwxLjQsMi4xMUEyLjI5MSwyLjI5MSwwLDAsMCwxMzYuMTI0LDE2My4zMjhabS0uOSwzLjU0M2ExLjQzMywxLjQzMywwLDEsMSwxLjQzNC0xLjQzM0ExLjQzMywxLjQzMywwLDAsMSwxMzUuMjI3LDE2Ni44NzFaIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtMTI4LjA2MSAtMTM5LjkxNSkiIGZpbGw9InVybCgjbGluZWFyLWdyYWRpZW50LTQpIi8+Cjwvc3ZnPgo=",
        id: "nami"
    }
];

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
        return result.concat(TEMP_WALLETS);
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
