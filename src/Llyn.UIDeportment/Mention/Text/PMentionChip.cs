using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed record PMentionChip(
    long PMentionChipId, string PMentionChipWord, string PMentionChipName, string PMentionChipSense)
{
    internal static void PMentionChipApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PMentionChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionWord") is TextBlock word)
        {
            word.Text = chip.PMentionChipWord;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionName") is TextBlock name)
        {
            name.Text = chip.PMentionChipName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMentionSense") is TextBlock sense)
        {
            sense.Text = chip.PMentionChipSense;
        }

        if (QLook.QLookPartFind<Button>(container, "PMentionUnlink") is not Button unlink)
        {
            return;
        }

        unlink.Command = PMentionCommand.PMentionCommandUnlink;
        unlink.CommandParameter = chip;
        if (QLook.QLookPartFind<QIconImage>(unlink, "PMentionMark") is QIconImage mark)
        {
            mark.QIconSource = QIcon.QIconResolve("close", 12);
        }
    }
}
