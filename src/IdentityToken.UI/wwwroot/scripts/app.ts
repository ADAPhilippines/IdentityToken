import CardanoWasmLoader from './CardanoWasmLoader'
import { CardanoWalletInterop } from "./CardanoWalletInterop";


declare global {
	interface Window { CardanoWalletInterop: CardanoWalletInterop; }
}

class Program {
	public static Main(): void {
		window.CardanoWalletInterop = new CardanoWalletInterop();
	}
}

Program.Main();