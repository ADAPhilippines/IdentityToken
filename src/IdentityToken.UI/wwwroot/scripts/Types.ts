type Paginate = {
    page: number,
    limit: number,
};

type ProtocolParameters = {
    min_fee_a: number,
    min_fee_b: number,
    min_utxo: number,
    pool_deposit: number,
    key_deposit: number,
    max_tx_size: number
}

type Block = {
    time: number,
    height: number,
    hash: string,
    slot: number,
    epoch: number,
    epoch_slot: number,
    slot_leader: string,
    size: number,
    tx_count: number,
}

export { 
    Paginate, 
    ProtocolParameters, 
    Block 
}