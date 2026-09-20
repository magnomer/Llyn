using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LParadigmRow(
    IReadOnlyList<LParadigmSlot> LParadigmRowSlots,
    bool LParadigmRowLead)
{
    public LParadigmSlot LParadigmRowFirst => LParadigmRowSlots[0];

    public string LParadigmRowName =>
        string.Join(", ", LParadigmRowSlots.Select(static slot => slot.LParadigmSlotMorphology.LMorphologyName));

    public string LParadigmRowPart =>
        LParadigmRowLead ? LParadigmRowFirst.LParadigmSlotSpeech.LSpeechValueName : string.Empty;

    public static IReadOnlyList<LParadigmRow> LParadigmRowScan(IReadOnlyList<LParadigmSlot> slots)
    {
        ArgumentNullException.ThrowIfNull(slots);

        HashSet<long> parts = [];
        foreach (LParadigmSlot slot in slots)
        {
            parts.Add(slot.LParadigmSlotSpeech.LSpeechValueId);
        }

        List<LParadigmRow> rows = [];
        List<LParadigmSlot> row = [];
        long? previous = null;
        foreach (LParadigmSlot slot in slots)
        {
            if (row.Count > 0 && !row[0].LParadigmSlotMatch(slot))
            {
                rows.Add(LParadigmRowCreate(row, parts.Count, previous));
                previous = row[0].LParadigmSlotSpeech.LSpeechValueId;
                row = [];
            }

            row.Add(slot);
        }

        if (row.Count > 0)
        {
            rows.Add(LParadigmRowCreate(row, parts.Count, previous));
        }

        return rows;
    }

    private static LParadigmRow LParadigmRowCreate(List<LParadigmSlot> row, int parts, long? previous)
    {
        return new LParadigmRow(row, parts > 1 && previous != row[0].LParadigmSlotSpeech.LSpeechValueId);
    }
}
