using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Llyn.Application;

namespace Llyn.UIVeneer;

public sealed class PLocalizationCatalog : INotifyPropertyChanged
{
    public static PLocalizationCatalog PLocalizationCatalogCurrent { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => PLocalizationTextFind(key) ?? string.Empty;

    internal static string PLocalizationTextRead(string key)
    {
        return PLocalizationTextFind(key) ?? key;
    }

    internal static string? PLocalizationTextFind(string key)
    {
        return System.Windows.Application.Current?.TryFindResource(key) as string;
    }

    internal static void PLocalizationCatalogApply(ResourceDictionary resources, string language)
    {
        foreach ((string key, string value) in LLocalization.LLocalizationLoad(language))
        {
            resources[key] = value;
        }

        PLocalizationCatalogCurrent.PropertyChanged?.Invoke(
            PLocalizationCatalogCurrent,
            new PropertyChangedEventArgs("Item[]"));
    }
}
