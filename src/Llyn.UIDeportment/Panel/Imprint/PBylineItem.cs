using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PBylineItem
{
    internal PBylineItem(long id, string name, string word)
    {
        PBylineItemId = id;

        int found = -1;
        int size = 0;
        if (word.Length != 0)
        {
            found = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                name, word, CompareOptions.IgnoreCase, out size);
        }

        if (found < 0)
        {
            PBylineItemLead = name;
            PBylineItemMark = string.Empty;
            PBylineItemTail = string.Empty;
            return;
        }

        PBylineItemLead = name[..found];
        PBylineItemMark = name.Substring(found, size);
        PBylineItemTail = name[(found + size)..];
    }

    internal long PBylineItemId { get; }

    public string PBylineItemLead { get; }

    public string PBylineItemMark { get; }

    public string PBylineItemTail { get; }

    internal static long? PBylineItemRead(object? chosen)
    {
        return (chosen as PBylineItem)?.PBylineItemId;
    }

    internal static void PBylineItemApply(FrameworkElement container, object item, MouseButtonEventHandler press)
    {
        if (item is not PBylineItem byline)
        {
            return;
        }

        if (PLook.PLookPartFind<Grid>(container, "PBylineRow") is Grid row)
        {
            row.PreviewMouseLeftButtonDown -= press;
            row.PreviewMouseLeftButtonDown += press;
        }

        if (PLook.PLookPartFind<Run>(container, "PBylineLead") is Run lead)
        {
            lead.Text = byline.PBylineItemLead;
        }

        if (PLook.PLookPartFind<Run>(container, "PBylineMark") is Run mark)
        {
            mark.Text = byline.PBylineItemMark;
        }

        if (PLook.PLookPartFind<Run>(container, "PBylineTail") is Run tail)
        {
            tail.Text = byline.PBylineItemTail;
        }
    }

    internal static IReadOnlyList<PBylineItem> PBylineItemBuild(IReadOnlyList<LAuthor> rows, string word)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(word);

        List<PBylineItem> built = new(rows.Count);
        foreach (LAuthor author in rows)
        {
            built.Add(new PBylineItem(author.LAuthorId, author.LAuthorName, word));
        }

        return built;
    }
}
