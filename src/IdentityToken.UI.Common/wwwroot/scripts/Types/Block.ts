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

export default Block;