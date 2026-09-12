using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly HashSet<long> _lEngineDraftHeld = [];

    private readonly HashSet<long> _lEngineDraftStale = [];

    public LDraft LEngineDraftStart(string origin, long? entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            long entry = entryId is null or <= 0 ? 0 : entryId.Value;
            LEntryDraft content = entry == 0
                ? new LEntryDraft(string.Empty, string.Empty, null, string.Empty, [], [])
                : LEngineEntryLoad(entry) ?? throw new LRefusal(LRefusal.LRefusalEntry);

            LDraft draft = new(
                LEngineIdentityCreate(),
                origin,
                entry,
                content,
                DateTimeOffset.UtcNow);

            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
            LClaimArchive.LClaimArchiveSave(
                _lEngineWorkspace, LClaimArchive.LClaimArchiveCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    public LEntryDraft LEngineDraftSave(LDraft draft)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(draft);
            LEngineDraftValidate(draft.LDraftId);

            LEntryDraft content = LEngineDraftNormalize(draft.LDraftContent);
            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftContent = content });
            return content;
        }
    }

    public LDraft? LEngineDraftRead(long id)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            return LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id);
        }
    }

    public IReadOnlyList<LDraft> LEngineDraftScan()
    {
        lock (_lEngineGate)
        {
            return LDraftArchive.LDraftArchiveScan(_lEngineWorkspace);
        }
    }

    public void LEngineDraftDelete(long id)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            LEngineCourtRemove(id);
            _lEngineDraftHeld.Remove(id);
            _lEngineTrove.LTroveClear(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        }
    }

    public bool LEngineDraftCheck(long id)
    {
        lock (_lEngineGate)
        {
            if (id == 0)
            {
                return false;
            }

            LEngineDraftValidate(id);

            LDraft? draft = LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id);
            if (draft is null)
            {
                return false;
            }

            if (draft.LDraftExample is LExample sentence)
            {
                return LEngineExampleCheck(draft, sentence);
            }

            if (draft.LDraftSituation is LSituation situation)
            {
                return LEngineSituationCheck(draft, situation);
            }

            if (draft.LDraftReference is LReference reference)
            {
                return LEngineReferenceCheck(draft, reference);
            }

            LEntryDraft origin = draft.LDraftEntryId <= 0
                ? LEngineDraftBlank with { LEntryDraftLanguage = draft.LDraftContent.LEntryDraftLanguage }
                : LEngineEntryLoad(draft.LDraftEntryId) ?? LEngineDraftBlank;

            return !LEngineDraftMatch(origin, draft.LDraftContent);
        }
    }

    public LOutcome LEngineDraftCommit(long id)
    {
        LOutcome outcome;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            outcome = LEngineDraftCommit(id, [], true);
            _lEngineTrove.LTroveClear(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, outcome.LOutcomeEntry.LEntryId);
        return outcome;
    }

    private LOutcome LEngineDraftCommit(long id, HashSet<long> entered, bool held)
    {
        entered.Add(id);
        LDraft draft = LEngineDraftLoad(id);
        Dictionary<long, long> identity = [];

        foreach (LCourt link in LEngineCourtScan(id))
        {
            if (entered.Contains(link.LCourtTargetId) ||
                LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtTargetId) is null)
            {
                continue;
            }

            LOutcome target = LEngineDraftCommit(
                link.LCourtTargetId, entered, LEngineHoldCheck(link.LCourtTargetId));
            LEngineIdentityRecord(identity, link.LCourtTargetId, target.LOutcomeEntry.LEntryId);
        }

        LEntryDraft sending = LEngineTranslationSettle(draft.LDraftContent, identity);

        LEntry entry = draft.LDraftEntryId <= 0
            || LEngineEntryLoad(draft.LDraftEntryId) is null
            ? LEngineEntrySave(sending, identity)
            : LEngineEntryUpdate(draft.LDraftEntryId, sending, identity);

        LDraft settled = draft with { LDraftEntryId = entry.LEntryId };
        if (LEngineEntryLoad(entry.LEntryId) is LEntryDraft written)
        {
            settled = settled with { LDraftContent = written };
        }

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, settled);

        foreach (LCourt link in
            LCourtArchive.LCourtArchiveSettle(_lEngineWorkspace, id))
        {
            LEngineCourtUpdate(link, entry.LEntryId);
        }

        if (!held)
        {
            return new LOutcome(entry, identity);
        }

        _lEngineDraftHeld.Remove(id);
        LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
        LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        return new LOutcome(entry, identity);
    }

    public void LEngineDraftCancel(long id)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            LEngineCourtRemove(id);

            foreach (LCourt link in LCourtArchive.LCourtArchiveSettle(_lEngineWorkspace, id))
            {
                LEngineCourtUpdate(link, 0);
            }

            _lEngineDraftHeld.Remove(id);
            _lEngineTrove.LTroveClear(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        }
    }

    private bool LEngineHoldCheck(long id)
    {
        LClaim? claim = LClaimArchive.LClaimArchiveCheck(_lEngineWorkspace, id)
            ? LClaimArchive.LClaimArchiveRead(_lEngineWorkspace, id)
            : null;

        return claim is not null
            && claim.LClaimProcess == Environment.ProcessId
            && _lEngineDraftHeld.Contains(id);
    }

    private bool LEngineClaimCheck(long id)
    {
        if (!LClaimArchive.LClaimArchiveCheck(_lEngineWorkspace, id))
        {
            return false;
        }

        LClaim? claim = LClaimArchive.LClaimArchiveRead(_lEngineWorkspace, id);
        return claim is not null && claim.LClaimProcess != Environment.ProcessId;
    }

    private static LEntryDraft LEngineDraftBlank =>
        new(string.Empty, string.Empty, null, string.Empty, [], []);

    private void LEngineDraftValidate(long id)
    {
        if (_lEngineDraftStale.Contains(id))
        {
            throw new LRefusal(LRefusal.LRefusalStale);
        }
    }

    private LDraft LEngineDraftLoad(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);

        return LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id)
            ?? throw new LRefusal(LRefusal.LRefusalDraft);
    }

    private LEntryDraft LEngineTranslationSettle(
        LEntryDraft content, IReadOnlyDictionary<long, long> identity)
    {
        return content with
        {
            LEntryDraftMeanings = LEngineTranslationSettle(content.LEntryDraftMeanings, identity),
            LEntryDraftCollocations = LEngineTranslationSettle(content.LEntryDraftCollocations, identity),
        };
    }

    private IReadOnlyList<LCardDraft> LEngineTranslationSettle(
        IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<long, long> identity)
    {
        List<LCardDraft> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<long> translations = new(card.LCardDraftTranslation.Count);
            foreach (long held in card.LCardDraftTranslation)
            {
                long translation = identity.TryGetValue(held, out long settled) ? settled : held;
                if (translation > 0 && LEngineEntryLoad(translation) is not null)
                {
                    translations.Add(translation);
                }
            }

            written.Add(card with
            {
                LCardDraftTranslation = translations,
                LCardDraftChild = LEngineTranslationSettle(card.LCardDraftChild, identity),
            });
        }

        return written;
    }
}
