using System;
using System.Collections.Generic;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class PBootstrap : System.Windows.Application
{
    public PBootstrap()
    {
        Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Llyn.UIVeneer;component/App.xaml"),
        });
    }

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
        window.PWindowSurface.Show();
    }
}
