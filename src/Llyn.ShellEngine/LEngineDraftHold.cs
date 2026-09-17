using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly HashSet<long> _lEngineDraftHeld = [];

    private readonly HashSet<long> _lEngineDraftStale = [];

    internal LDraft LEngineDraftStart(string origin, long? entryId)
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
            LEngineChronicleClear(id);
            _lEngineTrove.LTroveClear(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    internal bool LEngineDraftCheck(long id)
    {
        return LEngineDraftCheck(id, out _);
    }

    internal bool LEngineDraftCheck(long id, out string? refusal)
    {
        lock (_lEngineGate)
        {
            refusal = null;
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

            refusal = LEngineRefusalRead(draft);
            return LEngineDraftCheck(draft);
        }
    }

    private bool LEngineDraftCheck(LDraft draft)
    {
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

    public void LEngineDraftSweep(long id)
    {
        LDraft saved;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            saved = LEngineDraftLoad(id).LDraftNormalize();
            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, saved);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
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
            List<Action> written = [];

            using (LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart())
            {
                outcome = LEngineDraftCommit(id, true, loaded, settled, deferred, finished, written);
                LEngineCourtApply(loaded, settled, deferred);
                session.LDatabaseSessionCommit();
            }

            foreach (Action step in written)
            {
                step();
            }

            foreach (long done in finished)
            {
                _lEngineDraftHeld.Remove(done);
                LEngineChronicleClear(done);
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
            LEngineInflectionStart(entryId);
        }

        return outcome;
    }

    private LOutcome LEngineDraftCommit(
        long id,
        bool held,
        Dictionary<long, LDraft> loaded,
        Dictionary<long, LOutcome> settled,
        List<LCourt> deferred,
        List<long> finished,
        List<Action> written)
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
                finished,
                written);
            LEngineIdentityRecord(identity, link.LCourtTargetId, target.LOutcomeEntry.LEntryId);
        }

        LEntryDraft sending = LEngineTranslationSettle(draft.LDraftContent, identity);

        LEntry entry = draft.LDraftEntryId <= 0
            || LEngineEntryLoad(draft.LDraftEntryId) is null
            ? LEngineEntrySave(sending, identity)
            : LEngineEntryUpdate(draft.LDraftEntryId, sending, identity);

        LOutcome outcome = new(entry, identity);
        settled[id] = outcome;

        LDraft saved = draft with { LDraftEntryId = entry.LEntryId };
        if (LEngineEntryLoad(entry.LEntryId) is LEntryDraft stored)
        {
            saved = saved with { LDraftContent = stored };
        }

        long entryId = entry.LEntryId;
        written.Add(() => LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, saved));
        written.Add(() =>
        {
            foreach (LCourt link in LCourtArchive.LCourtArchiveSettle(_lEngineWorkspace, id))
            {
                LEngineCourtUpdate(link, entryId);
            }
        });

        if (held)
        {
            finished.Add(id);
        }

        return outcome;
    }

    internal void LEngineDraftCancel(long id)
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
            LEngineChronicleClear(id);
            _lEngineTrove.LTroveClear(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
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

    private static string? LEngineRefusalRead(LDraft draft)
    {
        if (draft.LDraftExample is not null
            || draft.LDraftSituation is not null
            || draft.LDraftReference is not null)
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(draft.LDraftContent.LEntryDraftHeadword)
            ? LRefusal.LRefusalHeadword
            : null;
    }

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
}
