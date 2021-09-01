import CardanoWasmLoader from './CardanoWasmLoader'
import { CardanoWalletInterop } from "./CardanoWalletInterop";


declare global {
	interface Window { CardanoWalletInterop: CardanoWalletInterop; }
}

window.CardanoWalletInterop = new CardanoWalletInterop();