using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class QLocalizationCatalog : INotifyPropertyChanged
{
    private static readonly ConditionalWeakTable<ResourceDictionary, ResourceDictionary> QLocalizationCatalogMerged =
        [];

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
        if (texts.All(pair => Equals(resources[pair.Key], pair.Value)))
        {
            return;
        }

        ResourceDictionary catalog = new();
        QLocalizationCatalogMerged.TryGetValue(resources, out ResourceDictionary? previous);
        if (previous is not null)
        {
            foreach (DictionaryEntry entry in previous)
            {
                catalog[entry.Key] = entry.Value;
            }
        }

        foreach ((string key, string value) in texts)
        {
            catalog[key] = value;
        }

        int index = previous is null ? -1 : resources.MergedDictionaries.IndexOf(previous);
        if (index < 0)
        {
            resources.MergedDictionaries.Add(catalog);
        }
        else
        {
            resources.MergedDictionaries[index] = catalog;
        }

        QLocalizationCatalogMerged.AddOrUpdate(resources, catalog);
        QLocalizationCatalogCurrent.PropertyChanged?.Invoke(
            QLocalizationCatalogCurrent,
            new PropertyChangedEventArgs("Item[]"));
    }
}
