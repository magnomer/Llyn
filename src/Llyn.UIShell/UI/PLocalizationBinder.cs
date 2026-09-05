using System.Windows;
using System.Windows.Data;

namespace Llyn.UIShell;

public sealed class PLocalizationBinder : Binding
{
    private string _pLocalizationBinderKey = string.Empty;

    public PLocalizationBinder()
    {
        Source = PLocalizationCatalog.PLocalizationCatalogCurrent;
        Mode = BindingMode.OneWay;
        FallbackValue = string.Empty;
    }

    public string PLocalizationBinderKey
    {
        get => _pLocalizationBinderKey;
        set
        {
            _pLocalizationBinderKey = value;
            Path = new PropertyPath($"[{value}]");
        }
    }
}
