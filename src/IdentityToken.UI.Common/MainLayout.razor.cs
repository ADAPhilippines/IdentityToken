using System.Threading.Tasks;
using IdentityToken.UI.Common.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common
{
    public partial class MainLayout
    {
        [Inject] private IJSRuntime? JsRuntime { get; set; }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (JsRuntime != null)
                {
                    var bootstrapInterop = new BootstrapInterop(JsRuntime);
                    await bootstrapInterop.InjectGoogleFontAsync("https://fonts.googleapis.com/css2?family=Poppins:wght@100;200;300;400;500;600;700;800;900&display=swap");
                    await bootstrapInterop.InjectStyleSheetAsync("./_content/IdentityToken.UI.Common/dist/app.css");
                }
            
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}