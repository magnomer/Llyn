using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PLanguageItem
{
    internal PLanguageItem(string name, ImageSource? flag)
    {
        PLanguageItemName = name;
        PLanguageItemFlag = flag;
    }

    public string PLanguageItemName { get; }

    public ImageSource? PLanguageItemFlag { get; }

    internal bool PLanguageItemMatch(string language)
    {
        return string.Equals(PLanguageItemName, language, StringComparison.Ordinal);
    }

    internal static void PLanguageItemReset(ObservableCollection<PLanguageItem> rows, IReadOnlyList<string> languages)
    {
        rows.Clear();
        foreach (string language in languages)
        {
            rows.Add(new PLanguageItem(language, PEnsign.PEnsignFind(language)));
        }
    }

    internal static string PLanguageNameRead(object sender)
    {
        return PSender.PSenderItemRead<PLanguageItem>(sender)?.PLanguageItemName ?? string.Empty;
    }
}
