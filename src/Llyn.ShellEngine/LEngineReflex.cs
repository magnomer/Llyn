using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LReflexRule> LEngineReflexRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? [] : LEngineLanguageLoad(language).LLanguageReflexRules;
    }

    public IReadOnlyList<LReflex> LEngineReflexRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return new LReflexArchive(_lEngineDatabase).LReflexRead(entryId);
        }
    }

    internal IReadOnlyList<LReflex> LEngineReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(reflexes);
            IReadOnlyList<LReflex> saved = new LReflexArchive(_lEngineDatabase).LReflexSet(entryId, reflexes);
            LEngineEpithetUpdate(entryId);
            LEngineUpdatedSet(entryId);
            return saved;
        }
    }

    private void LEngineReflexSync(
        long entryId,
        string language,
        IReadOnlyList<LReflexDraft> drafts,
        List<LRevisionChange>? changes,
        Dictionary<long, long> identity)
    {
        LReflexArchive reflexes = new(_lEngineDatabase);
        IReadOnlyList<LReflex> stored = reflexes.LReflexRead(entryId);
        IReadOnlyList<LReflexDraft> written = LEngineAnatomyScan(language, LEngineReflexScan(drafts));
        IReadOnlyList<LReflex> current = LEngineReflexRead(entryId, written);

        if (LEngineReflexMatch(stored, current))
        {
            return;
        }

        foreach (LReflex row in current)
        {
            if (row.LReflexId > 0 && !stored.Any(kept => kept.LReflexId == row.LReflexId))
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }
        }

        IReadOnlyList<LReflex> saved = reflexes.LReflexSet(entryId, current);
        LEngineEpithetUpdate(entryId);
        for (int index = 0; index < saved.Count; index++)
        {
            LEngineIdentityRecord(identity, written[index].LReflexDraftId, saved[index].LReflexId);
        }

        if (LEngineReflexMatch(LEngineAnatomyClear(stored), LEngineAnatomyClear(current)))
        {
            return;
        }

        changes?.Add(new LRevisionChange(
            0,
            entryId,
            "reflex",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            LEngineReflexFormat(current)));
    }

    private static IReadOnlyList<LReflexDraft> LEngineReflexScan(IReadOnlyList<LReflexDraft> drafts)
    {
        List<LReflexDraft> filled = [];
        foreach (LReflexDraft draft in drafts)
        {
            if (!draft.LReflexDraftEmpty)
            {
                filled.Add(draft);
            }
        }

        return filled;
    }

    private static IReadOnlyList<LReflex> LEngineReflexRead(long entryId, IReadOnlyList<LReflexDraft> drafts)
    {
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
                draft.LReflexDraftNote.Trim(),
                draft.LReflexDraftRespelling.Trim(),
                draft.LReflexDraftRegion.Trim(),
                draft.LReflexDraftRemark.Trim(),
                draft.LReflexDraftAnatomy,
                draft.LReflexDraftAnchors));
        }

        return rows;
    }

    private static bool LEngineReflexMatch(IReadOnlyList<LReflex> stored, IReadOnlyList<LReflex> current)
    {
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

    private static bool LEngineReflexMatch(IReadOnlyList<LReflexDraft> one, IReadOnlyList<LReflexDraft> other)
    {
        one = [.. LEngineReflexScan(one)];
        other = [.. LEngineReflexScan(other)];
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!LAnchor.LAnchorMatch(one[index].LReflexDraftAnchors, other[index].LReflexDraftAnchors)
                || one[index] with { LReflexDraftAnchors = [] } != other[index] with { LReflexDraftAnchors = [] })
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<LReflexDraft> LEngineReflexReset(IReadOnlyList<LReflexDraft> drafts)
    {
        List<LReflexDraft> renewed = new(drafts.Count);
        foreach (LReflexDraft draft in drafts)
        {
            renewed.Add(draft.LReflexDraftId > 0 ? draft with { LReflexDraftId = 0 } : draft);
        }

        return renewed;
    }

    private IReadOnlyList<LReflexDraft> LEngineReflexNormalize(IReadOnlyList<LReflexDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static draft => draft.LReflexDraftId == 0 && draft.LReflexDraftEmpty,
            draft => draft.LReflexDraftId == 0 ? draft with { LReflexDraftId = LEngineIdentityCreate() } : draft);
    }

    private static string LEngineReflexFormat(IReadOnlyList<LReflex> reflexes)
    {
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
            reflex.LReflexLanguage, reflex.LReflexRegion, reflex.LReflexKind, reflex.LReflexText, reflex.LReflexNote,
            reflex.LReflexRemark,
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
