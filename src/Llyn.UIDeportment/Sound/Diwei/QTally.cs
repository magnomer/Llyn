using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTally
{
    private QTally(CTally tally)
    {
        QTallyLanguage = tally.CTallyLanguage;
        QTallyKind = tally.CTallyKind;
        QTallyMarks = QTallyMark.QTallyMarkBuild(tally.CTallyMarks);
    }

    public string QTallyLanguage { get; }

    public string QTallyKind { get; }

    public IReadOnlyList<QTallyMark> QTallyMarks { get; }

    internal static void QTallyItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QTally tally)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PTallyLanguage") is TextBlock language)
        {
            language.Text = tally.QTallyLanguage;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PTallyKind") is TextBlock kind)
        {
            kind.Text = tally.QTallyKind;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PTallyMarks") is ItemsControl marks)
        {
            marks.ItemsSource = tally.QTallyMarks;
            QLookItem.QLookItemAttach(marks, QTallyMark.QTallyMarkRefine);
        }
    }

    internal static IReadOnlyList<QTally> QTallyBuild(IReadOnlyList<CTally> tallies)
    {
        ArgumentNullException.ThrowIfNull(tallies);

        return QSplice.QSpliceBuild(tallies, static tally => new QTally(tally));
    }
}
