using System;
using System.Collections.Generic;

namespace Llyn.UIVeneer;

public partial class PBootstrap : System.Windows.Application
{
    public void PBootstrapThemeApply(Func<string, string> colorRead, IReadOnlyDictionary<string, string> catalog)
    {
        PLocalizationCatalog.PLocalizationCatalogApply(Resources, catalog);
        PThemeLoader.PThemeLoaderApply(colorRead, Resources);
        PField.PFieldApply(Resources);
        PIndicator.PIndicatorApply(Resources);
        PCaret.PCaretHook();
    }

    public void PBootstrapCatalogApply(IReadOnlyDictionary<string, string> catalog)
    {
        PLocalizationCatalog.PLocalizationCatalogApply(Resources, catalog);
    }

    public string PBootstrapTextRead(string key)
    {
        return PLocalizationCatalog.PLocalizationTextRead(key);
    }

    public void PBootstrapWindowShow(PWindow window)
    {
        window.Show();
    }
}
