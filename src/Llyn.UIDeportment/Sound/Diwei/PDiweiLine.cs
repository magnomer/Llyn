using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PDiweiLine
{
    private PDiweiLine(LDiweiLine line)
    {
        PDiweiLineReading = line.LDiweiLineReading;
        PDiweiLineLabel = line.LDiweiLineLabel;
        PDiweiLineRounded = line.LDiweiLineRounded;
        PDiweiLineCharacters = line.LDiweiLineCharacters;
    }

    public string PDiweiLineReading { get; }

    public string PDiweiLineLabel { get; }

    public bool PDiweiLineRounded { get; }

    public IReadOnlyList<string> PDiweiLineCharacters { get; }

    internal static void PDiweiLineApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PDiweiLine line)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PDiweiReading") is TextBlock reading)
        {
            reading.Text = line.PDiweiLineReading;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PDiweiLabel") is TextBlock label)
        {
            label.Text = line.PDiweiLineLabel;
        }

        if (PLook.PLookPartFind<Border>(container, "PDiweiRounded") is Border rounded)
        {
            rounded.Visibility = PLook.PLookVisibleRead(line.PDiweiLineRounded);
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PDiweiCharacters") is ItemsControl characters)
        {
            characters.ItemsSource = line.PDiweiLineCharacters;
        }
    }

    internal static IReadOnlyList<PDiweiLine> PDiweiLineBuild(IReadOnlyList<LDiweiLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        List<PDiweiLine> built = new(lines.Count);
        foreach (LDiweiLine line in lines)
        {
            built.Add(new PDiweiLine(line));
        }

        return built;
    }
}
