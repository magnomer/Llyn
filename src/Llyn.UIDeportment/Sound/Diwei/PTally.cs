using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PTally
{
    private PTally(LTallyLine line, bool respelled)
    {
        PTallyLanguage = line.LTallyLineLanguage;
        PTallyKind = line.LTallyLineKind;
        PTallyMarks = PTallyMark.PTallyMarkBuild(line.LTallyLineRead(respelled));
    }

    public string PTallyLanguage { get; }

    public string PTallyKind { get; }

    public IReadOnlyList<PTallyMark> PTallyMarks { get; }

    internal static void PTallyApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PTally tally)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PTallyLanguage") is TextBlock language)
        {
            language.Text = tally.PTallyLanguage;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PTallyKind") is TextBlock kind)
        {
            kind.Text = tally.PTallyKind;
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PTallyMarks") is ItemsControl marks)
        {
            marks.ItemsSource = tally.PTallyMarks;
            PLookItem.PLookItemAttach(marks, PTallyMark.PTallyMarkApply);
        }
    }

    internal static IReadOnlyList<PTally> PTallyBuild(IReadOnlyList<LTallyLine> lines, bool respelled)
    {
        ArgumentNullException.ThrowIfNull(lines);

        List<PTally> built = new(lines.Count);
        foreach (LTallyLine line in lines)
        {
            built.Add(new PTally(line, respelled));
        }

        return built;
    }
}
