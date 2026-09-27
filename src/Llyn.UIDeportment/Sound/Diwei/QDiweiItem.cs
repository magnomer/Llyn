using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDiweiItem
{
    private QDiweiItem(CDiweiSection section)
    {
        QDiweiItemLabel = section.CDiweiSectionLabel;
        QDiweiItemLines = QDiweiLine.QDiweiLineBuild(section.CDiweiSectionLines);
        QDiweiItemTallies = QTally.QTallyBuild(section.CDiweiSectionTallies);
        QDiweiItemSwitched = section.CDiweiSectionSwitched;
        QDiweiItemRespelled = section.CDiweiSectionRespelled;
    }

    public string QDiweiItemLabel { get; }

    public IReadOnlyList<QDiweiLine> QDiweiItemLines { get; }

    public IReadOnlyList<QTally> QDiweiItemTallies { get; }

    public bool QDiweiItemSwitched { get; }

    public bool QDiweiItemRespelled { get; }

    internal static void QDiweiItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QDiweiItem section)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PDiweiHead") is TextBlock head)
        {
            head.Text = section.QDiweiItemLabel;
        }

        if (QLook.QLookPartFind<Border>(container, "PDiweiSwitch") is Border choice)
        {
            choice.Visibility = QLook.QLookVisibleRead(section.QDiweiItemSwitched);
        }

        QDiweiChoiceApply(
            container,
            "PDiweiSpoken",
            section.QDiweiItemRespelled
                ? QContract.QContractSheetFind<Style>("Theme.Diwei.Ipa")
                : QContract.QContractSheetFind<Style>("Theme.Diwei.IpaChosen"));
        QDiweiChoiceApply(
            container,
            "PDiweiSpelled",
            section.QDiweiItemRespelled
                ? QContract.QContractSheetFind<Style>("Theme.Diwei.RespellingChosen")
                : QContract.QContractSheetFind<Style>("Theme.Diwei.Respelling"));
        if (QLook.QLookPartFind<ItemsControl>(container, "PDiweiTally") is ItemsControl tally)
        {
            tally.ItemsSource = section.QDiweiItemTallies;
            QLookItem.QLookItemAttach(tally, QTally.QTallyApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PDiweiLines") is ItemsControl lines)
        {
            lines.ItemsSource = section.QDiweiItemLines;
            QLookItem.QLookItemAttach(lines, QDiweiLine.QDiweiLineApply);
        }
    }

    private static void QDiweiChoiceApply(FrameworkElement container, string name, Style sheet)
    {
        if (QLook.QLookPartFind<Button>(container, name) is Button choice)
        {
            choice.Style = sheet;
        }
    }

    internal static IReadOnlyList<QDiweiItem> QDiweiItemBuild(IReadOnlyList<CDiweiSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        return LSplice.LSpliceBuild(sections, static section => new QDiweiItem(section));
    }
}
