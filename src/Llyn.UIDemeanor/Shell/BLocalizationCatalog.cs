using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

public sealed class BLocalizationCatalog
{
    private IReadOnlyDictionary<string, string> _bLocalizationCatalogText =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public string BLocalizationTextRead(string key)
    {
        return _bLocalizationCatalogText.TryGetValue(key, out string? text) ? text : key;
    }

    internal void BLocalizationCatalogApply(IReadOnlyDictionary<string, string> texts)
    {
        ArgumentNullException.ThrowIfNull(texts);

        Dictionary<string, string> catalog = new(_bLocalizationCatalogText, StringComparer.Ordinal);
        foreach ((string key, string value) in texts)
        {
            catalog[key] = value;
        }

        _bLocalizationCatalogText = catalog;
    }
}
