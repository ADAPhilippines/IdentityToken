interface ICardanoWalletInteropObjectRef {
    invokeMethodAsync(methodName: string, args: any) : Promise<void>
}

export default ICardanoWalletInteropObjectRef;