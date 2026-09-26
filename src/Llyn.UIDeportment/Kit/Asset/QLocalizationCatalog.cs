using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class QLocalizationCatalog : INotifyPropertyChanged
{
    public static QLocalizationCatalog QLocalizationCatalogCurrent { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => QLocalizationTextFind(key) ?? string.Empty;

    public static string QLocalizationTextRead(string key)
    {
        return QLocalizationTextFind(key) ?? key;
    }

    public static string? QLocalizationTextFind(string key)
    {
        return System.Windows.Application.Current?.TryFindResource(key) as string;
    }

    internal static void QLocalizationCatalogApply(
        ResourceDictionary resources, IReadOnlyDictionary<string, string> texts)
    {
        foreach ((string key, string value) in texts)
        {
            resources[key] = value;
        }

        QLocalizationCatalogCurrent.PropertyChanged?.Invoke(
            QLocalizationCatalogCurrent,
            new PropertyChangedEventArgs("Item[]"));
    }
}
