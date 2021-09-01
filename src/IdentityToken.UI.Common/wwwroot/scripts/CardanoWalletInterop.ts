import CardanoWasmLoader from "./Helpers/CardanoWasmLoader";
import ProtocolParameters from "./Types/ProtocolParameters";
import Block from "./Types/Block";
import Utils from "./Helpers/Utils";
import {
    Address,
    BaseAddress,
    Ed25519KeyHash,
    Mint,
    NativeScript,
    Value
} from "@emurgo/cardano-serialization-lib-browser";
import {Buffer} from "Buffer";

class CardanoWalletInterop {
    private static blockfrostPID: string;
    private static isMainnet: boolean;

    constructor() {
    }

    private static get BlockfrostBaseURL() {
        return CardanoWalletInterop.isMainnet ? "https://cardano-mainnet.blockfrost.io/api/v0" : "https://cardano-testnet.blockfrost.io/api/v0";
    }

    public static async IsWalletConnectedAsync(): Promise<boolean> {
        return await window.cardano.isEnabled();
    }

    public static async ConnectWalletAsync(): Promise<boolean> {
        return await window.cardano.enable();
    }

    private static CreateTxMint(assetName: string, script: NativeScript): Mint {
        const mintAssets = CardanoWasmLoader.Cardano.MintAssets.new();
        mintAssets.insert(
            CardanoWasmLoader.Cardano.AssetName.new(Buffer.from(assetName)),
            CardanoWasmLoader.Cardano.Int.new(CardanoWasmLoader.Cardano.BigNum.from_str("1"))
        );

        const mint = CardanoWasmLoader.Cardano.Mint.new();
        mint.insert(
            CardanoWasmLoader.Cardano.ScriptHash.from_bytes(
                script
                    .hash(CardanoWasmLoader.Cardano.ScriptHashNamespace.NativeScript)
                    .to_bytes()
            ),
            mintAssets
        );
        return mint;
    }

    private static CalculateMintValue(assetName: string, script: NativeScript): Value {
        const mintedAssets = CardanoWasmLoader.Cardano.Assets.new();
        mintedAssets.insert(
            CardanoWasmLoader.Cardano.AssetName.new(Buffer.from(assetName)),
            CardanoWasmLoader.Cardano.BigNum.from_str("1")
        );

        const multiAsset = CardanoWasmLoader.Cardano.MultiAsset.new();
        multiAsset.insert(
            CardanoWasmLoader.Cardano.ScriptHash.from_bytes(script.hash(0).to_bytes()),
            mintedAssets
        );

        const mintedValue = CardanoWasmLoader.Cardano.Value.new(CardanoWasmLoader.Cardano.BigNum.from_str("0"));
        mintedValue.set_multiasset(multiAsset);

        let value = CardanoWasmLoader.Cardano.Value.new(CardanoWasmLoader.Cardano.BigNum.from_str("0"));
        value = value.checked_add(mintedValue);
        return value;
    }

    private static CreatePolicyScript = (address: Address): { policyId: string, script: NativeScript } => {
        const pKeyHash = (CardanoWasmLoader.Cardano.BaseAddress.from_address(address) as BaseAddress)
            .payment_cred()
            .to_keyhash() as Ed25519KeyHash;

        const script = CardanoWasmLoader.Cardano.ScriptPubkey.new(pKeyHash);
        const nativeScript = CardanoWasmLoader.Cardano.NativeScript.new_script_pubkey(script);
        const scriptHash = CardanoWasmLoader.Cardano.ScriptHash.from_bytes(nativeScript.hash(0).to_bytes());
        const policyId = Buffer.from(scriptHash.to_bytes()).toString("hex");

        return {policyId, script: nativeScript}
    }

    public MintIdentityTokenAsync = async (assetName: string): Promise<void> => {
        const addressHex = (await window.cardano.getUsedAddresses())[0];
        const addressBuffer = Buffer.from(addressHex, "hex");
        const address = CardanoWasmLoader.Cardano.Address.from_bytes(addressBuffer);

        let {policyId, script} = CardanoWalletInterop.CreatePolicyScript(address);

        const nativeScripts = CardanoWasmLoader.Cardano.NativeScripts.new();
        nativeScripts.add(script);

        let outputValue = CardanoWasmLoader.Cardano.Value.new(CardanoWasmLoader.Cardano.BigNum.from_str("0"));
        const mintValue = CardanoWalletInterop.CalculateMintValue(assetName, script);
        outputValue = outputValue.checked_add(mintValue);

        const utxosHex = await window.cardano.getUtxos();
        const inputs = CardanoWasmLoader.Cardano.TransactionInputs.new();
        utxosHex.forEach((item) => {
            const utxo = CardanoWasmLoader.Cardano.TransactionUnspentOutput.from_bytes(Buffer.from(item, "hex"));
            inputs.add(
                CardanoWasmLoader.Cardano.TransactionInput.new(
                    utxo.input().transaction_id(),
                    utxo.input().index()
                )
            );
            outputValue = outputValue.checked_add(utxo.output().amount());
        });

        const rawOutputs = CardanoWasmLoader.Cardano.TransactionOutputs.new();
        rawOutputs.add(
            CardanoWasmLoader.Cardano.TransactionOutput.new(
                address,
                outputValue
            )
        );

        const latestBlock = await this.GetLatestBlockAsync();
        const rawTxBody = CardanoWasmLoader.Cardano.TransactionBody.new(
            inputs,
            rawOutputs,
            CardanoWasmLoader.Cardano.BigNum.from_str("0"),
            latestBlock.slot + 1000
        );

        const mint = CardanoWalletInterop.CreateTxMint(assetName, script);
        rawTxBody.set_mint(mint);
    }

    public async GetFromBlockfrostAsync<T>(endpoint: string): Promise<T | null> {
        const response = await fetch(`${CardanoWalletInterop.BlockfrostBaseURL}/${endpoint}`, {
            headers: {
                "project_id": CardanoWalletInterop.blockfrostPID
            }
        });
        const responseBody = await response.json();
        if (responseBody.error) {
            return null;
        } else {
            return responseBody;
        }
    }

    public async InitializeAsync(blockfrost_pid: string, isMainnet: boolean = true): Promise<void> {
        await CardanoWasmLoader.Load();
        CardanoWalletInterop.blockfrostPID = blockfrost_pid;
        CardanoWalletInterop.isMainnet = isMainnet;

        //Test
        if (!await CardanoWalletInterop.IsWalletConnectedAsync()) {
            await CardanoWalletInterop.ConnectWalletAsync();
        }
        await this.MintIdentityTokenAsync("TestId");
    }

    private async GetProtocolParametersAsync(epoch: number): Promise<ProtocolParameters> {
        let protocolParameters: ProtocolParameters | null;

        while (true) {
            protocolParameters = await this.GetFromBlockfrostAsync<ProtocolParameters>(`epochs/${epoch}/parameters`);
            if (protocolParameters != null) {
                break;
            } else {
                await Utils.Delay(1000);
            }
        }
        return protocolParameters;
    }

    private async GetLatestBlockAsync(): Promise<Block> {
        let latestBlock: Block | null;
        while (true) {
            latestBlock = await this.GetFromBlockfrostAsync<Block>("blocks/latest");
            if (latestBlock != null) {
                break;
            } else {
                await Utils.Delay(1000);
            }
        }
        return latestBlock;
    }
}

export default CardanoWalletInterop;