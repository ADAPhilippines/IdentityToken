import CardanoWalletInterop from "./CardanoWalletInterop";
import Helper from "./Helpers/Helper";

window.CardanoWalletInterop = new CardanoWalletInterop();
window.ScrollToElementBottom = Helper.ScrollToElementBottom;
window.GetElementScrollTop = Helper.GetElementScrollTop;
window.ScrollToMessageId = Helper.ScrollToMessageId;
window.GenerateQRDataUrlAsync = Helper.GenerateQRDataUrlAsync;
window.CopyToClipboardAsync = Helper.CopyToClipboardAsync;