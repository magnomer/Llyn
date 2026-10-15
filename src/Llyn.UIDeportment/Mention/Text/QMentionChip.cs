using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed record QMentionChip(
    long QMentionChipId, string QMentionChipWord, string QMentionChipName, string QMentionChipSense)
{
    internal static IReadOnlyList<QMentionChip> QMentionChipCreate(IReadOnlyList<CMentionLabel> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        List<QMentionChip> chips = new(labels.Count);
        foreach (CMentionLabel label in labels)
        {
            chips.Add(new QMentionChip(
                label.CMentionLabelId,
                label.CMentionLabelWord,
                label.CMentionLabelKey is string key
                    ? QLocalizationCatalog.QLocalizationTextRead(key)
                    : label.CMentionLabelName,
                label.CMentionLabelSense));
        }

        return chips;
    }

    internal static void QMentionChipRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QMentionChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionWord") is TextBlock word)
        {
            word.Text = chip.QMentionChipWord;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionName") is TextBlock name)
        {
            name.Text = chip.QMentionChipName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionSense") is TextBlock sense)
        {
            sense.Text = chip.QMentionChipSense;
        }

        if (QLook.QLookPartFind<Button>(container, "PMentionUnlink") is not Button unlink)
        {
            return;
        }

        unlink.Command = QMentionCommand.QMentionCommandUnlink;
        unlink.CommandParameter = chip;
        if (QLook.QLookPartFind<QIconImage>(unlink, "PMentionMark") is QIconImage mark)
        {
            mark.QIconSource = QIcon.QIconResolve("close", 12);
        }
    }
}
