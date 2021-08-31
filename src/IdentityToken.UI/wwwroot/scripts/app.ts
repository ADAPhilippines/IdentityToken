import WasmLoader from './wasm-loader'

interface ICardanoInterop {
	Greeting: string;
}

declare global {
	interface Window { CardanoInterop: ICardanoInterop; }
}

class Program {
	public static Main(): void {
		window.CardanoInterop = {
			Greeting: "Hello, World"
		};
        console.log(window.CardanoInterop.Greeting);
        Program.InitializeAsync();
	}

    public static async InitializeAsync() : Promise<void>
    {
        await WasmLoader.LoadModules();
    }
}

Program.Main();