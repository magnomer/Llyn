using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PDiweiItem
{
    private PDiweiItem(LDiweiSection section)
    {
        PDiweiItemLabel = section.LDiweiSectionLabel;
        PDiweiItemLines = PDiweiLine.PDiweiLineBuild(section.LDiweiSectionLines);
        PDiweiItemTallies = PTally.PTallyBuild(section.LDiweiSectionTallies, section.LDiweiSectionRespelled);
        PDiweiItemSwitched = section.LDiweiSectionSwitched;
        PDiweiItemRespelled = section.LDiweiSectionRespelled;
    }

    public string PDiweiItemLabel { get; }

    public IReadOnlyList<PDiweiLine> PDiweiItemLines { get; }

    public IReadOnlyList<PTally> PDiweiItemTallies { get; }

    public bool PDiweiItemSwitched { get; }

    public bool PDiweiItemRespelled { get; }

    internal static void PDiweiItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PDiweiItem section)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PDiweiHead") is TextBlock head)
        {
            head.Text = section.PDiweiItemLabel;
        }

        if (PLook.PLookPartFind<Border>(container, "PDiweiSwitch") is Border choice)
        {
            choice.Visibility = PLook.PLookVisibleRead(section.PDiweiItemSwitched);
        }

        PDiweiChoiceApply(container, "PDiweiSpoken", !section.PDiweiItemRespelled);
        PDiweiChoiceApply(container, "PDiweiSpelled", section.PDiweiItemRespelled);
        if (PLook.PLookPartFind<ItemsControl>(container, "PDiweiTally") is ItemsControl tally)
        {
            tally.ItemsSource = section.PDiweiItemTallies;
            PLookItem.PLookItemAttach(tally, PTally.PTallyApply);
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PDiweiLines") is ItemsControl lines)
        {
            lines.ItemsSource = section.PDiweiItemLines;
            PLookItem.PLookItemAttach(lines, PDiweiLine.PDiweiLineApply);
        }
    }

    private static void PDiweiChoiceApply(FrameworkElement container, string name, bool chosen)
    {
        if (PLook.PLookPartFind<Button>(container, name) is not Button choice)
        {
            return;
        }

        if (chosen)
        {
            choice.SetResourceReference(Control.BackgroundProperty, "Theme.AccentSoft");
            choice.SetResourceReference(Control.ForegroundProperty, "Theme.Accent");
            return;
        }

        choice.ClearValue(Control.BackgroundProperty);
        choice.ClearValue(Control.ForegroundProperty);
    }

    internal static IReadOnlyList<PDiweiItem> PDiweiItemBuild(IReadOnlyList<LDiweiSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        List<PDiweiItem> built = new(sections.Count);
        foreach (LDiweiSection section in sections)
        {
            built.Add(new PDiweiItem(section));
        }

        return built;
    }
}
