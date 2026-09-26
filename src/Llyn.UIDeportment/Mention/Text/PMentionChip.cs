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

        if (PLook.PLookPartFind<TextBlock>(container, "PMentionWord") is TextBlock word)
        {
            word.Text = chip.PMentionChipWord;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PMentionName") is TextBlock name)
        {
            name.Text = chip.PMentionChipName;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PMentionSense") is TextBlock sense)
        {
            sense.Text = chip.PMentionChipSense;
        }

        if (PLook.PLookPartFind<Button>(container, "PMentionUnlink") is not Button unlink)
        {
            return;
        }

        unlink.Command = PMentionCommand.PMentionCommandUnlink;
        unlink.CommandParameter = chip;
        if (PLook.PLookPartFind<PIconImage>(unlink, "PMentionMark") is PIconImage mark)
        {
            mark.PIconSource = PIcon.PIconResolve("close", 12);
        }
    }
}
