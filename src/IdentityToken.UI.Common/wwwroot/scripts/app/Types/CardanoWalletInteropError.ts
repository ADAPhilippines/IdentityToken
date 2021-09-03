import CardanoWalletInteropErrorType from "../Models/CardanoWalletInteropErrorType";

type CardanoWalletInteropError = {
    type: CardanoWalletInteropErrorType;
    message: string;
}

export default CardanoWalletInteropError;