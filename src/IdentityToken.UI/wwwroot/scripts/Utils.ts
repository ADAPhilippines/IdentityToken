class Utils {
    constructor(){
        
    }
    
    public static async Delay(time: number): Promise<void>
    {
        return new Promise((resolve => {
            setTimeout(() => {resolve()}, time);
        }));
    }
}

export { Utils }