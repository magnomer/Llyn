using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LReflexClerkRow
{
    public static IReadOnlyList<LReflexDraft> LReflexRoundResolve(IReadOnlyList<IReadOnlyList<LReflexDraft>> rounds)
    {
        ArgumentNullException.ThrowIfNull(rounds);

        List<LReflexDraft> merged = [];
        List<int> stamps = [];
        for (int round = 0; round < rounds.Count; round++)
        {
            foreach (LReflexDraft row in rounds[round])
            {
                int place = merged.FindIndex(held =>
                    string.Equals(held.LReflexDraftLanguage, row.LReflexDraftLanguage, StringComparison.Ordinal)
                    && string.Equals(held.LReflexDraftRegion, row.LReflexDraftRegion, StringComparison.Ordinal)
                    && string.Equals(held.LReflexDraftKind, row.LReflexDraftKind, StringComparison.Ordinal)
                    && string.Equals(
                        held.LReflexDraftRomanization,
                        row.LReflexDraftRomanization,
                        StringComparison.Ordinal));
                if (place >= 0 && stamps[place] != round)
                {
                    LReflexDraft held = merged[place];
                    merged[place] = held with
                    {
                        LReflexDraftText = held.LReflexDraftText + ' ' + row.LReflexDraftText,
                        LReflexDraftMain = held.LReflexDraftMain || row.LReflexDraftMain,
                    };
                    stamps[place] = round;
                    continue;
                }

                merged.Add(row);
                stamps.Add(round);
            }
        }

        return merged;
    }

    public static IReadOnlyList<LReflex> LReflexRowRead(long entryId, IReadOnlyList<LReflexDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LReflex> rows = [];
        foreach (LReflexDraft draft in drafts)
        {
            rows.Add(new LReflex(
                Math.Max(draft.LReflexDraftId, 0),
                entryId,
                rows.Count,
                draft.LReflexDraftLanguage.Trim(),
                draft.LReflexDraftKind.Trim(),
                draft.LReflexDraftText.Trim(),
                draft.LReflexDraftMain,
                draft.LReflexDraftRomanization.Trim(),
                draft.LReflexDraftMeaning.Trim(),
                draft.LReflexDraftOwned,
                draft.LReflexDraftNote.Trim(),
                draft.LReflexDraftRespelling.Trim(),
                draft.LReflexDraftRegion.Trim(),
                draft.LReflexDraftAnatomy,
                draft.LReflexDraftAnchors));
        }

        return rows;
    }

    public static bool LReflexRowMatch(IReadOnlyList<LReflex> stored, IReadOnlyList<LReflex> current)
    {
        ArgumentNullException.ThrowIfNull(stored);
        ArgumentNullException.ThrowIfNull(current);

        if (stored.Count != current.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.Count; index++)
        {
            if (!LAnchor.LAnchorMatch(stored[index].LReflexAnchors, current[index].LReflexAnchors)
                || stored[index] with { LReflexAnchors = [] } != current[index] with { LReflexAnchors = [] })
            {
                return false;
            }
        }

        return true;
    }

    public static IReadOnlyList<LReflex> LReflexAnatomyClear(IReadOnlyList<LReflex> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<LReflex> bare = new(rows.Count);
        foreach (LReflex row in rows)
        {
            bare.Add(row with { LReflexAnatomy = LAnatomy.LAnatomyEmpty });
        }

        return bare;
    }

    public static string LReflexRowFormat(IReadOnlyList<LReflex> reflexes)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        List<string> lines = new(reflexes.Count);
        foreach (LReflex reflex in reflexes)
        {
            lines.Add(string.Join(' ', LReflexPartScan(reflex)));
        }

        return string.Join(", ", lines);
    }

    private static IEnumerable<string> LReflexPartScan(LReflex reflex)
    {
        string[] parts =
        [
            reflex.LReflexLanguage, reflex.LReflexRegion, reflex.LReflexKind, reflex.LReflexText,
            reflex.LReflexRomanization, reflex.LReflexMeaning, reflex.LReflexNote,
        ];
        foreach (string part in parts)
        {
            if (part.Length > 0)
            {
                yield return part;
            }
        }
    }
}
