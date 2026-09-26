using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace Llyn.UIDeportment;

internal sealed class PWindowScreen
{
    private Task<CoreWebView2Environment>? _pScreenSetting;

    internal Task<CoreWebView2Environment> PScreenSettingRead()
    {
        return _pScreenSetting ??= CoreWebView2Environment.CreateAsync(
            null,
            null,
            new CoreWebView2EnvironmentOptions
            {
                AdditionalBrowserArguments = "--autoplay-policy=no-user-gesture-required",
            });
    }

    internal void PScreenSettingReset()
    {
        _pScreenSetting = null;
    }
}
