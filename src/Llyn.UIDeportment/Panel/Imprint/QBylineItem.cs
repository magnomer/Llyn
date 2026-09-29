using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QBylineItem
{
    internal QBylineItem(long id, string lead, string mark, string tail)
    {
        QBylineItemId = id;
        QBylineItemLead = lead;
        QBylineItemMark = mark;
        QBylineItemTail = tail;
    }

    internal long QBylineItemId { get; }

    public string QBylineItemLead { get; }

    public string QBylineItemMark { get; }

    public string QBylineItemTail { get; }

    internal static long? QBylineItemRead(object? chosen)
    {
        return (chosen as QBylineItem)?.QBylineItemId;
    }

    internal static void QBylineItemRefine(FrameworkElement container, object item, MouseButtonEventHandler press)
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

    internal static IReadOnlyList<QBylineItem> QBylineItemBuild(IReadOnlyList<CAuthor> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QBylineItem> built = new(rows.Count);
        foreach (CAuthor author in rows)
        {
            built.Add(new QBylineItem(author.CAuthorId, author.CAuthorLead, author.CAuthorMark, author.CAuthorTail));
        }

        return built;
    }
}
