import CardanoWalletInteropErrorType from "../Enums/CardanoWalletInteropErrorType";

type CardanoWalletInteropError = {
    type: CardanoWalletInteropErrorType;
    message: string;
}

export default CardanoWalletInteropError;