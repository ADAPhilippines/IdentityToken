/**
 * Loads the WASM modules
 */

class WasmLoader {
    private static _cardano: any;

    public async LoadModules() : Promise<void> {
        WasmLoader._cardano = await import("@emurgo/cardano-serialization-lib-browser/cardano_serialization_lib");
    }

    public get Cardano() {
        return WasmLoader._cardano;
    }
}

export default new WasmLoader();