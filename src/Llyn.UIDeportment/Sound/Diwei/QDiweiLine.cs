using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDiweiLine
{
    private QDiweiLine(CDiweiLine line)
    {
        QDiweiLineReading = line.CDiweiLineReading;
        QDiweiLineLabel = line.CDiweiLineLabel;
        QDiweiLineRounded = line.CDiweiLineRounded;
        QDiweiLineCharacters = line.CDiweiLineCharacters;
    }

    public string QDiweiLineReading { get; }

    public string QDiweiLineLabel { get; }

    public bool QDiweiLineRounded { get; }

    public IReadOnlyList<string> QDiweiLineCharacters { get; }

    internal static void QDiweiLineRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QDiweiLine line)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PDiweiReading") is TextBlock reading)
        {
            reading.Text = line.QDiweiLineReading;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PDiweiLabel") is TextBlock label)
        {
            label.Text = line.QDiweiLineLabel;
        }

        if (QLook.QLookPartFind<Border>(container, "PDiweiRounded") is Border rounded)
        {
            rounded.Visibility = QLook.QLookVisibleRead(line.QDiweiLineRounded);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PDiweiCharacters") is ItemsControl characters)
        {
            characters.ItemsSource = line.QDiweiLineCharacters;
        }
    }

    internal static IReadOnlyList<QDiweiLine> QDiweiLineBuild(IReadOnlyList<CDiweiLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        return LSplice.LSpliceBuild(lines, static line => new QDiweiLine(line));
    }
}
