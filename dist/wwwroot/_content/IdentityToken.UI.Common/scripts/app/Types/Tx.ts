import QuantifiedObject from "./QuantifiedObject";

type Tx = {
    hash: string,
    block: string,
    block_height: number,
    slot: number,
    index: number,
    output_amount: QuantifiedObject[],
    fees: number,
    deposit: number,
    size: number,
    invalid_before: null,
    invalid_hereafter: number,
    utxo_count: number,
    withdrawal_count: number,
    mir_cert_count: number,
    delegation_count: number,
    stake_cert_count: number,
    pool_update_count: number,
    pool_retire_count: number,
    asset_mint_or_burn_count: number
}

export default Tx;