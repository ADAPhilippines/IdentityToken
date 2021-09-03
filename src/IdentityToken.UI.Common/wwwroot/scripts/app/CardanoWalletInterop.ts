import CardanoWasmLoader from "./Helpers/CardanoWasmLoader";
import ProtocolParameters from "./Types/ProtocolParameters";
import Block from "./Types/Block";
import Utils from "./Helpers/Utils";
import {
    Address,
    BaseAddress,
    Ed25519KeyHash,
    MetadataHash,
    Mint,
    NativeScript,
    NativeScripts,
    Transaction,
    TransactionBody,
    TransactionMetadata,
    Value
} from "@emurgo/cardano-serialization-lib-browser";
import {Buffer} from "Buffer";
import ICardanoWalletInteropObjectRef from "./Interfaces/ICardanoWalletInteropObjectRef";
import CardanoWalletInteropErrorType from "./Enums/CardanoWalletInteropErrorType";
import CardanoWalletInteropError from "./Types/CardanoWalletInteropError";
import TxOutput from "./Types/TxOutput";
import Tx from "./Types/Tx";

class CardanoWalletInterop {
    private blockfrostProjectId: string = "";
    private isMainnet: boolean = true;
    private objectRef: ICardanoWalletInteropObjectRef | null = null;

    constructor() {
    }

    private get BlockfrostBaseURL() {
        return this.isMainnet ? "https://cardano-mainnet.blockfrost.io/api/v0" : "https://cardano-testnet.blockfrost.io/api/v0";
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

    private static CreatePolicyScript(address: Address): { policyId: string, script: NativeScript } {
        const pKeyHash = (CardanoWasmLoader.Cardano.BaseAddress.from_address(address) as BaseAddress)
            .payment_cred()
            .to_keyhash() as Ed25519KeyHash;

        const script = CardanoWasmLoader.Cardano.ScriptPubkey.new(pKeyHash);
        const nativeScript = CardanoWasmLoader.Cardano.NativeScript.new_script_pubkey(script);
        const scriptHash = CardanoWasmLoader.Cardano.ScriptHash.from_bytes(nativeScript.hash(0).to_bytes());
        const policyId = Buffer.from(scriptHash.to_bytes()).toString("hex");

        return {policyId, script: nativeScript}
    }

    private static async CalculateTxFeeAsync(
        txBody: TransactionBody,
        numWitness: number,
        protocolParams: ProtocolParameters,
        txMetadata: TransactionMetadata | undefined = undefined,
        nativeScripts: NativeScripts | null = null): Promise<Value> {
        const dummyVkeyWitness =
            "8258208814c250f40bfc74d6c64f02fc75a54e68a9a8b3736e408d9820a6093d5e38b95840f04a036fa56b180af6537b2bba79cec75191dc47419e1fd8a4a892e7d84b7195348b3989c15f1e7b895c5ccee65a1931615b4bdb8bbbd01e6170db7a6831310c";

        const vkeys = CardanoWasmLoader.Cardano.Vkeywitnesses.new();
        for (let i = 0; i < numWitness; i++) {
            vkeys.add(
                CardanoWasmLoader.Cardano.Vkeywitness.from_bytes(
                    Buffer.from(dummyVkeyWitness, "hex")
                )
            );
        }

        const dummyWitnesses = CardanoWasmLoader.Cardano.TransactionWitnessSet.new();
        dummyWitnesses.set_vkeys(vkeys);
        if (nativeScripts !== null)
            dummyWitnesses.set_scripts(nativeScripts);

        const rawTx = CardanoWasmLoader.Cardano.Transaction.new(
            txBody,
            dummyWitnesses,
            txMetadata
        );

        let minFee = CardanoWasmLoader.Cardano.min_fee(rawTx, CardanoWasmLoader.Cardano.LinearFee.new(
            CardanoWasmLoader.Cardano.BigNum.from_str(protocolParams.min_fee_a.toString()),
            CardanoWasmLoader.Cardano.BigNum.from_str(protocolParams.min_fee_b.toString())
        ));

        return CardanoWasmLoader.Cardano.Value.new(minFee);
    }

    public async IsWalletConnectedAsync(): Promise<boolean> {
        return await window.cardano.isEnabled();
    }

    public async ConnectWalletAsync(): Promise<boolean> {
        let result = false;
        try {
            result = await window.cardano.enable();
        } catch (e: any) {
            console.error("Connect Wallet Error: ", e);
            let err: CardanoWalletInteropError = {
                type: CardanoWalletInteropErrorType.connectWalletError,
                message: e
            }
            await this.ThrowErrorAsync(err);
        }
        return result;
    }

    public async MintIdentityTokenAsync(assetName: string, metadata: string): Promise<Tx | null> {
        let result: Tx | null = null;
        const transaction = await this.CreateMintTx(assetName, metadata);
        if (transaction !== null) {
            const signedTx = await this.signTxAsync(transaction);
            if (signedTx !== null) {
                let txHash = await this.SubmitTxAsync(signedTx);
                if(txHash !== null)
                    result = await this.GetTransactionAsync(txHash)
            }
        }
        return result;
    }

    public async SendAdaAsync(outputs: TxOutput[]): Promise<Tx | null> {
        let result: Tx | null = null;
        const transaction = await this.CreateNormalTx(outputs);
        if (transaction !== null) {
            const signedTx = await this.signTxAsync(transaction);
            if (signedTx != null) {
                let txHash = await this.SubmitTxAsync(signedTx);
                if(txHash !== null)
                    result = await this.GetTransactionAsync(txHash)
            }
        }
        
        return result;
    }

    public async InitializeAsync(blockfrost_pid: string, objectRef: ICardanoWalletInteropObjectRef, isMainnet: boolean = true): Promise<void> {
        await CardanoWasmLoader.Load();
        this.blockfrostProjectId = blockfrost_pid;
        this.isMainnet = isMainnet;
        this.objectRef = objectRef;
    }

    private async signTxAsync(transaction: Transaction): Promise<Transaction | null> {
        let result: Transaction | null = null;
        try {
            const transactionHex = Buffer.from(transaction.to_bytes()).toString("hex");
            const witnesses = await window.cardano.signTx(transactionHex);

            const txWitnesses = transaction.witness_set();
            const txVkeys = txWitnesses.vkeys();
            const txScripts = txWitnesses.scripts();

            const addWitnesses = CardanoWasmLoader.Cardano.TransactionWitnessSet.from_bytes(
                Buffer.from(witnesses, "hex")
            );

            const addVkeys = addWitnesses.vkeys();
            const addScripts = addWitnesses.scripts();

            const totalVkeys = CardanoWasmLoader.Cardano.Vkeywitnesses.new();
            const totalScripts = CardanoWasmLoader.Cardano.NativeScripts.new();

            if (txVkeys) {
                for (let i = 0; i < txVkeys.len(); i++) {
                    totalVkeys.add(txVkeys.get(i));
                }
            }
            if (txScripts) {
                for (let i = 0; i < txScripts.len(); i++) {
                    totalScripts.add(txScripts.get(i));
                }
            }
            if (addVkeys) {
                for (let i = 0; i < addVkeys.len(); i++) {
                    totalVkeys.add(addVkeys.get(i));
                }
            }
            if (addScripts) {
                for (let i = 0; i < addScripts.len(); i++) {
                    totalScripts.add(addScripts.get(i));
                }
            }

            const totalWitnesses = CardanoWasmLoader.Cardano.TransactionWitnessSet.new();
            totalWitnesses.set_vkeys(totalVkeys);
            totalWitnesses.set_scripts(totalScripts);

            result = CardanoWasmLoader.Cardano.Transaction.new(
                transaction.body(),
                totalWitnesses,
                transaction.metadata()
            );
        } catch (e: any) {
            console.error("Error in signing Tx:", e)
            let err: CardanoWalletInteropError = {
                type: CardanoWalletInteropErrorType.signTxError,
                message: e.info
            }
            await this.ThrowErrorAsync(err);
        }
        return result;
    }

    private async CreateNormalTx(outputs: TxOutput[]): Promise<Transaction | null> {
        try {
            const MAX_INPUTS = 20;

            const latestBlock = await this.GetLatestBlockAsync();
            let protocolParams = await this.GetProtocolParametersAsync(latestBlock.epoch);

            const txBuilder = CardanoWasmLoader.Cardano.TransactionBuilder.new(
                CardanoWasmLoader.Cardano.LinearFee.new(
                    CardanoWasmLoader.Cardano.BigNum.from_str(protocolParams.min_fee_a.toString()),
                    CardanoWasmLoader.Cardano.BigNum.from_str(protocolParams.min_fee_b.toString())),
                CardanoWasmLoader.Cardano.BigNum.from_str(protocolParams.min_utxo.toString()),
                CardanoWasmLoader.Cardano.BigNum.from_str(protocolParams.pool_deposit.toString()),
                CardanoWasmLoader.Cardano.BigNum.from_str(protocolParams.key_deposit.toString()));

            const utxosHex = await window.cardano.getUtxos();
            let counter = 0;
            utxosHex.every((item) => {
                if (counter < MAX_INPUTS) {
                    const utxo = CardanoWasmLoader.Cardano.TransactionUnspentOutput.from_bytes(Buffer.from(item, "hex"));
                    txBuilder.add_input(
                        utxo.output().address(),
                        utxo.input(),
                        utxo.output().amount());
                    counter++;
                    return true;
                } else {
                    return false;
                }
            });

            outputs.forEach((output) => {
                txBuilder.add_output(
                    CardanoWasmLoader.Cardano.TransactionOutput.new(
                        CardanoWasmLoader.Cardano.Address.from_bech32(output.address),
                        CardanoWasmLoader.Cardano.Value.new(CardanoWasmLoader.Cardano.BigNum.from_str(output.amount.toString()))
                    )
                );
            });

            txBuilder.set_ttl(latestBlock.slot + 1000);

            const addressHex = (await window.cardano.getUsedAddresses())[0];
            const addressBuffer = Buffer.from(addressHex, "hex");
            const address = CardanoWasmLoader.Cardano.Address.from_bytes(addressBuffer);
            txBuilder.add_change_if_needed(address);

            const txBody = txBuilder.build();

            const transaction = CardanoWasmLoader.Cardano.Transaction.new(
                txBody,
                CardanoWasmLoader.Cardano.TransactionWitnessSet.new(), // witnesses
                undefined, // transaction metadata
            );

            if (transaction.to_bytes().length * 2 > protocolParams.max_tx_size)
                throw Error("Transaction is too big");

            return transaction;
        } catch (e: any) {
            console.error("Error in Creating Tx:", e);
            let err: CardanoWalletInteropError = {
                type: CardanoWalletInteropErrorType.createTxError,
                message: e
            }
            await this.ThrowErrorAsync(err);
            return null;
        }
    }


    private async CreateMintTx(assetName: string, metadata: string): Promise<Transaction | null> {
        try {
            const MAX_INPUTS = 20;
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
            utxosHex.every((item) => {
                if (inputs.len() < MAX_INPUTS) {
                    const utxo = CardanoWasmLoader.Cardano.TransactionUnspentOutput.from_bytes(Buffer.from(item, "hex"));
                    inputs.add(
                        CardanoWasmLoader.Cardano.TransactionInput.new(
                            utxo.input().transaction_id(),
                            utxo.input().index()
                        )
                    );
                    outputValue = outputValue.checked_add(utxo.output().amount());
                    return true;
                } else {
                    return false;
                }
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

            metadata = `{\"${assetName}\":${metadata}}`;
            metadata = `{\"${policyId}\":${metadata}}`;
            const generalMetadata = CardanoWasmLoader.Cardano.GeneralTransactionMetadata.new();
            generalMetadata.insert(
                CardanoWasmLoader.Cardano.BigNum.from_str("7368"),
                CardanoWasmLoader.Cardano.encode_json_str_to_metadatum(metadata, 0)
            );

            let _metadata = CardanoWasmLoader.Cardano.TransactionMetadata.new(generalMetadata);
            rawTxBody.set_metadata_hash(CardanoWasmLoader.Cardano.hash_metadata(_metadata));

            let protocolParams = await this.GetProtocolParametersAsync(latestBlock.epoch);
            let fee = await CardanoWalletInterop.CalculateTxFeeAsync(rawTxBody, 2, protocolParams, _metadata, nativeScripts);
            outputValue = outputValue.checked_sub(fee);

            const outputs = CardanoWasmLoader.Cardano.TransactionOutputs.new();
            outputs.add(
                CardanoWasmLoader.Cardano.TransactionOutput.new(
                    address,
                    outputValue
                )
            );

            const finalTxBody = CardanoWasmLoader.Cardano.TransactionBody.new(
                inputs,
                outputs,
                fee.coin(),
                latestBlock.slot + 1000
            );

            finalTxBody.set_mint(rawTxBody.multiassets() as Mint);
            finalTxBody.set_metadata_hash(rawTxBody.metadata_hash() as MetadataHash);

            const finalWitnesses = CardanoWasmLoader.Cardano.TransactionWitnessSet.new();
            finalWitnesses.set_scripts(nativeScripts);

            _metadata = CardanoWasmLoader.Cardano.TransactionMetadata.new(generalMetadata);
            let transaction = CardanoWasmLoader.Cardano.Transaction.new(
                finalTxBody,
                finalWitnesses,
                _metadata
            );

            if (transaction.to_bytes().length * 2 > protocolParams.max_tx_size)
                throw Error("Transaction is too big");

            return transaction;
        } catch (e: any) {
            console.error("Error in Creating Mint Tx:", e);
            let err: CardanoWalletInteropError = {
                type: CardanoWalletInteropErrorType.createTxError,
                message: e
            }
            await this.ThrowErrorAsync(err);
            return null;
        }
    }

    private async GetFromBlockfrostAsync<T>(endpoint: string): Promise<T | null> {
        const response = await fetch(`${this.BlockfrostBaseURL}/${endpoint}`, {
            headers: {
                "project_id": this.blockfrostProjectId
            }
        });
        const responseBody = await response.json();
        if (responseBody.error) {
            return null;
        } else {
            return responseBody;
        }
    }

    private async SubmitTxAsync(transaction: Transaction): Promise<string | null> {
        const response = await fetch(`${this.BlockfrostBaseURL}/tx/submit`, {
            headers: {
                "project_id": this.blockfrostProjectId,
                "Content-Type": "application/cbor"
            },
            method: "POST",
            body: Buffer.from(transaction.to_bytes())
        });
        const responseBody = await response.json();
        if (responseBody.error) {
            console.error(responseBody);
            await this.ThrowErrorAsync(responseBody);
            return null;
        } else {
            return responseBody;
        }
    }

    private async ThrowErrorAsync(e: CardanoWalletInteropError): Promise<void> {
        if (this.objectRef) {
            await this.objectRef?.invokeMethodAsync("OnError", e);
        }
    }

    private async GetProtocolParametersAsync(epoch: number): Promise<ProtocolParameters> {
        let protocolParameters: ProtocolParameters | null;

        while (true) {
            protocolParameters = await this.GetFromBlockfrostAsync<ProtocolParameters>(`epochs/${epoch}/parameters`);
            if (protocolParameters !== null) {
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
            if (latestBlock !== null) {
                break;
            } else {
                await Utils.Delay(1000);
            }
        }
        return latestBlock;
    }

    private async GetTransactionAsync(hash: string): Promise<Tx | null> {
        let transaction: Tx | null;
        while (true) {
            transaction = await this.GetFromBlockfrostAsync<Tx>(`txs/${hash}`);
            if (transaction !== null) {
                break;
            } else {
                await Utils.Delay(3000);
            }
        }
        return transaction;
    }
}

export default CardanoWalletInterop;