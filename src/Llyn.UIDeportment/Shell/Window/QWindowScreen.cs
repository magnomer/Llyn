using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace Llyn.UIDeportment;

internal sealed class QWindowScreen
{
    private Task<CoreWebView2Environment>? _qScreenSetting;

    internal Task<CoreWebView2Environment> QScreenSettingRead()
    {
        return _qScreenSetting ??= CoreWebView2Environment.CreateAsync(
            null,
            null,
            new CoreWebView2EnvironmentOptions
            {
                AdditionalBrowserArguments = "--autoplay-policy=no-user-gesture-required",
            });
    }

    internal void QScreenSettingReset()
    {
        _qScreenSetting = null;
    }
}
