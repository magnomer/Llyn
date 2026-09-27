using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QBylineItem
{
    internal QBylineItem(long id, string name, string word)
    {
        QBylineItemId = id;

        int found = -1;
        int size = 0;
        if (word.Length != 0)
        {
            found = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                name, word, CompareOptions.IgnoreCase, out size);
        }

        if (found < 0)
        {
            QBylineItemLead = name;
            QBylineItemMark = string.Empty;
            QBylineItemTail = string.Empty;
            return;
        }

        QBylineItemLead = name[..found];
        QBylineItemMark = name.Substring(found, size);
        QBylineItemTail = name[(found + size)..];
    }

    internal long QBylineItemId { get; }

    public string QBylineItemLead { get; }

    public string QBylineItemMark { get; }

    public string QBylineItemTail { get; }

    internal static long? QBylineItemRead(object? chosen)
    {
        return (chosen as QBylineItem)?.QBylineItemId;
    }

    internal static void QBylineItemApply(FrameworkElement container, object item, MouseButtonEventHandler press)
    {
        if (item is not QBylineItem byline)
        {
            return;
        }

        if (QLook.QLookPartFind<Grid>(container, "PBylineRow") is Grid row)
        {
            row.PreviewMouseLeftButtonDown -= press;
            row.PreviewMouseLeftButtonDown += press;
        }

        if (QLook.QLookPartFind<Run>(container, "PBylineLead") is Run lead)
        {
            lead.Text = byline.QBylineItemLead;
        }

        if (QLook.QLookPartFind<Run>(container, "PBylineMark") is Run mark)
        {
            mark.Text = byline.QBylineItemMark;
        }

        if (QLook.QLookPartFind<Run>(container, "PBylineTail") is Run tail)
        {
            tail.Text = byline.QBylineItemTail;
        }
    }

    internal static IReadOnlyList<QBylineItem> QBylineItemBuild(IReadOnlyList<CAuthor> rows, string word)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(word);

        List<QBylineItem> built = new(rows.Count);
        foreach (CAuthor author in rows)
        {
            built.Add(new QBylineItem(author.CAuthorId, author.CAuthorName, word));
        }

        return built;
    }
}
