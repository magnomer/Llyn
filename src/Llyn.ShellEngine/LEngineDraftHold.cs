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
        List<long> raised = [];
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            Dictionary<long, LDraft> loaded = [];
            Dictionary<long, LOutcome> settled = [];
            List<LCourt> deferred = [];
            List<long> finished = [];

            using (LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart())
            {
                outcome = LEngineDraftCommit(id, true, loaded, settled, deferred, finished);
                LEngineCourtApply(loaded, settled, deferred);
                session.LDatabaseSessionCommit();
            }

            foreach (long done in finished)
            {
                _lEngineDraftHeld.Remove(done);
                LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, done);
                LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, done);
            }

            _lEngineTrove.LTroveClear(id);

            foreach (LOutcome made in settled.Values)
            {
                raised.Add(made.LOutcomeEntry.LEntryId);
            }
        }

        foreach (long entryId in raised)
        {
            LEngineBulletinRaise(LSubject.LSubjectEntry, entryId);
        }

        return outcome;
    }

    private LOutcome LEngineDraftCommit(
        long id,
        bool held,
        Dictionary<long, LDraft> loaded,
        Dictionary<long, LOutcome> settled,
        List<LCourt> deferred,
        List<long> finished)
    {
        LDraft draft = LEngineDraftLoad(id);
        loaded[id] = draft;
        Dictionary<long, long> identity = [];

        foreach (LCourt link in LEngineCourtScan(id))
        {
            if (loaded.ContainsKey(link.LCourtTargetId))
            {
                if (settled.TryGetValue(link.LCourtTargetId, out LOutcome? made))
                {
                    LEngineIdentityRecord(identity, link.LCourtTargetId, made.LOutcomeEntry.LEntryId);
                }
                else
                {
                    deferred.Add(link);
                }

                continue;
            }

            if (LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtTargetId) is null)
            {
                continue;
            }

            LOutcome target = LEngineDraftCommit(
                link.LCourtTargetId,
                LEngineHoldCheck(link.LCourtTargetId),
                loaded,
                settled,
                deferred,
                finished);
            LEngineIdentityRecord(identity, link.LCourtTargetId, target.LOutcomeEntry.LEntryId);
        }

        LEntryDraft sending = LEngineTranslationSettle(draft.LDraftContent, identity);

        LEntry entry = draft.LDraftEntryId <= 0
            || LEngineEntryLoad(draft.LDraftEntryId) is null
            ? LEngineEntrySave(sending, identity)
            : LEngineEntryUpdate(draft.LDraftEntryId, sending, identity);

        LOutcome outcome = new(entry, identity);
        settled[id] = outcome;

        LDraft written = draft with { LDraftEntryId = entry.LEntryId };
        if (LEngineEntryLoad(entry.LEntryId) is LEntryDraft stored)
        {
            written = written with { LDraftContent = stored };
        }

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, written);

        foreach (LCourt link in
            LCourtArchive.LCourtArchiveSettle(_lEngineWorkspace, id))
        {
            LEngineCourtUpdate(link, entry.LEntryId);
        }

        if (held)
        {
            finished.Add(id);
        }

        return outcome;
    }

    private void LEngineCourtApply(
        IReadOnlyDictionary<long, LDraft> loaded,
        IReadOnlyDictionary<long, LOutcome> settled,
        IReadOnlyList<LCourt> deferred)
    {
        foreach (LCourt link in deferred)
        {
            if (!loaded.TryGetValue(link.LCourtOwnerId, out LDraft? owner)
                || !settled.TryGetValue(link.LCourtOwnerId, out LOutcome? made)
                || !settled.TryGetValue(link.LCourtTargetId, out LOutcome? target))
            {
                continue;
            }

            long entryId = target.LOutcomeEntry.LEntryId;
            LEngineCourtApply(
                owner.LDraftContent.LEntryDraftMeanings, made, link.LCourtTargetId, entryId, false);
            LEngineCourtApply(
                owner.LDraftContent.LEntryDraftCollocations, made, link.LCourtTargetId, entryId, true);
        }
    }

    private void LEngineCourtApply(
        IReadOnlyList<LCardDraft> cards, LOutcome made, long draftId, long entryId, bool collocation)
    {
        foreach (LCardDraft card in cards)
        {
            if (LEngineTranslationCheck(card.LCardDraftTranslation, draftId))
            {
                long ownerId = made.LOutcomeIdentity.TryGetValue(card.LCardDraftId, out long real)
                    ? real
                    : card.LCardDraftId;
                if (ownerId > 0)
                {
                    LEngineTranslationAppend(ownerId, entryId, collocation);
                }
            }

            if (!collocation)
            {
                LEngineCourtApply(card.LCardDraftChild, made, draftId, entryId, false);
            }
        }
    }

    private static bool LEngineTranslationCheck(IReadOnlyList<long> translations, long id)
    {
        foreach (long held in translations)
        {
            if (held == id)
            {
                return true;
            }
        }

        return false;
    }

    private void LEngineTranslationAppend(long ownerId, long entryId, bool collocation)
    {
        LTranslationArchive translations = new(_lEngineDatabase);
        List<long> ids = [];
        foreach (LTranslation held in collocation
            ? translations.LTranslationCollocationRead(ownerId)
            : translations.LTranslationMeaningRead(ownerId))
        {
            ids.Add(held.LTranslationEntryId);
        }

        if (ids.Contains(entryId))
        {
            return;
        }

        ids.Add(entryId);
        LEngineTranslationSave(ownerId, ids, collocation);
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
        LEntryArchive entries = new(_lEngineDatabase);
        List<LCardDraft> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<long> translations = new(card.LCardDraftTranslation.Count);
            foreach (long held in card.LCardDraftTranslation)
            {
                long translation = identity.TryGetValue(held, out long settled) ? settled : held;
                if (translation > 0 && entries.LEntryRead(translation) is not null)
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
