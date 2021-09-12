interface IDotNetObjectRef {
    invokeMethodAsync(methodName: string, args: any) : Promise<void>;
}

export default IDotNetObjectRef;