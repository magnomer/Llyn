using System.ComponentModel;
using System.Windows;

namespace Llyn.UIShell;

public sealed class PLocalizationCatalog : INotifyPropertyChanged
{
    public static PLocalizationCatalog PLocalizationCatalogCurrent { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] =>
        System.Windows.Application.Current?.TryFindResource(key) as string ?? string.Empty;

    internal static void PLocalizationCatalogUpdate()
    {
        PLocalizationCatalogCurrent.PropertyChanged?.Invoke(
            PLocalizationCatalogCurrent,
            new PropertyChangedEventArgs("Item[]"));
    }
}
