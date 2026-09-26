using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class PLocalizationCatalog : INotifyPropertyChanged
{
    public static PLocalizationCatalog PLocalizationCatalogCurrent { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => PLocalizationTextFind(key) ?? string.Empty;

    internal static string PLocalizationTextRead(string key)
    {
        return LLocalizationCatalog.LLocalizationTextRead(key);
    }

    internal static string? PLocalizationTextFind(string key)
    {
        return LLocalizationCatalog.LLocalizationTextFind(key);
    }

    internal static void PLocalizationCatalogApply(
        ResourceDictionary resources, IReadOnlyDictionary<string, string> texts)
    {
        foreach ((string key, string value) in texts)
        {
            resources[key] = value;
        }

        PLocalizationCatalogCurrent.PropertyChanged?.Invoke(
            PLocalizationCatalogCurrent,
            new PropertyChangedEventArgs("Item[]"));
    }
}
