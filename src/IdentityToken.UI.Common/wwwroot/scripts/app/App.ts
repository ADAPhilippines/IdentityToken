import CardanoWalletInterop from "./CardanoWalletInterop";
import Helper from "./Helpers/Helper";
import {Bifrost} from "./Bifrost";

window.CardanoWalletInterop = new CardanoWalletInterop();
window.ScrollToElementBottom = Helper.ScrollToElementBottom;
window.GetElementScrollTop = Helper.GetElementScrollTop;
window.ScrollToMessageId = Helper.ScrollToMessageId;
window.GenerateQRDataUrlAsync = Helper.GenerateQRDataUrlAsync;
window.CopyToClipboardAsync = Helper.CopyToClipboardAsync;
window.AttachEmojiHandler = Helper.AttachEmojiHandler;
(window as any).test = Bifrost;