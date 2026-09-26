using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIDeportment;

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
            rows.Add(new PLanguageItem(language, LEnsignImage.LEnsignFind(language)));
        }
    }

    internal static void PLanguageItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PLanguageItem language)
        {
            return;
        }

        if (PLook.PLookPartFind<Image>(container, "PGlossOptionFlag") is Image flag)
        {
            flag.Source = language.PLanguageItemFlag;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PGlossOptionName") is TextBlock name)
        {
            name.Text = language.PLanguageItemName;
        }
    }

    internal static string PLanguageNameRead(object sender)
    {
        return PSender.PSenderItemRead<PLanguageItem>(sender)?.PLanguageItemName ?? string.Empty;
    }
}
