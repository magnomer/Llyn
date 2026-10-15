using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QLanguageItem
{
    internal QLanguageItem(string name, ImageSource? flag)
    {
        QLanguageItemName = name;
        QLanguageItemFlag = flag;
    }

    public string QLanguageItemName { get; }

    public ImageSource? QLanguageItemFlag { get; }

    internal static void QLanguageItemReset(ObservableCollection<QLanguageItem> rows, IReadOnlyList<string> languages)
    {
        rows.Clear();
        foreach (string language in languages)
        {
            rows.Add(new QLanguageItem(language, QEnsignImage.QEnsignRead(language)));
        }
    }

    internal static void QLanguageItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QLanguageItem language)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PGlossOptionFlag") is Image flag)
        {
            flag.Source = language.QLanguageItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGlossOptionName") is TextBlock name)
        {
            name.Text = language.QLanguageItemName;
        }
    }

    internal static string PLanguageNameRead(object sender)
    {
        return QSender.QSenderItemRead<QLanguageItem>(sender)?.QLanguageItemName ?? string.Empty;
    }
}
