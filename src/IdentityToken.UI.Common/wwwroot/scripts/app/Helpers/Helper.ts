import QRCode from 'qrcode';

class Helper {
    public static async Delay(time: number): Promise<void> {
        return new Promise((resolve => {
            setTimeout(() => {
                resolve()
            }, time);
        }));
    }
    
    public static ScrollToElementBottom(selector: string) {
        const element = document.querySelector(selector) as HTMLElement;
        
        if(element)
            element.scrollTop = element.scrollHeight;
    }
    
    public static GetElementScrollTop(selector: string) {
        const element = document.querySelector(selector) as HTMLElement;
        return element?.scrollTop ?? 0;
    }
    
    public static ScrollToMessageId(id: string) {
        const messageContainer = document.querySelector('#message-container') as HTMLElement;
        const targetMessageElement = document.querySelector(`[data-id="${id}"]`) as HTMLElement;
        
        if(messageContainer && targetMessageElement)
            messageContainer.scrollTop = targetMessageElement.offsetTop - 441;
    }
    
    public static async GenerateQRDataUrlAsync(data: string) {
        return await QRCode.toDataURL(data);
    }
    
    public static async CopyToClipboardAsync(data: string) {
        await navigator.clipboard.writeText(data);
    }
}

export default Helper;