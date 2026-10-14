using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEntryChip(LEntryChipKind LEntryChipKind, string LEntryChipText)
{
    public static IReadOnlyList<LEntryChip> LEntryChipRead(
        LUnit unit, LFreedom freedom, IReadOnlyList<LSpeechDraft> speeches)
    {
        ArgumentNullException.ThrowIfNull(speeches);

        List<LEntryChip> chips = [];
        if (unit != LUnit.LUnitEmpty && Enum.IsDefined(unit))
        {
            chips.Add(new LEntryChip(LEntryChipKind.LEntryChipUnit, LUnitKey.LUnitKeyRead(unit)));
        }

        if (freedom != LFreedom.LFreedomEmpty && Enum.IsDefined(freedom))
        {
            chips.Add(new LEntryChip(LEntryChipKind.LEntryChipFreedom, LFreedomKey.LFreedomKeyRead(freedom)));
        }

        foreach (LSpeechDraft speech in speeches)
        {
            if (speech.LSpeechDraftNamed)
            {
                chips.Add(new LEntryChip(LEntryChipKind.LEntryChipSpeech, speech.LSpeechDraftName));
            }
        }

        return chips;
    }
}
