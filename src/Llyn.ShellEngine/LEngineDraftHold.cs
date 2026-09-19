using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

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
            IReadOnlyList<string> languages = LEngineLanguageRead();
            LEntryDraft content = entry == 0
                ? LEngineDraftBlank with { LEntryDraftLanguage = languages.Count > 0 ? languages[0] : string.Empty }
                : LEngineEntryLoad(entry) ?? throw new LRefusal(LRefusal.LRefusalEntry);

            LDraft draft = new(
                LEngineIdentityCreate(),
                origin,
                entry,
                content,
                DateTimeOffset.UtcNow);

            _lEngineDrafts.LDraftSave(draft);
            _lEngineClaims.LClaimSave(_lEngineClaims.LClaimCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    internal IReadOnlyList<LRequest> LEngineDraftPrepare(long id)
    {
        lock (_lEngineGate)
        {
            LEntryDraft draft = LEngineDraftLoad(id).LDraftContent;
            List<LRequest> requests = [];
            if (!draft.LEntryDraftDefined)
            {
                requests.Add(new LRequestCardAddition(id, LCardKind.LCardKindMeaning, 0, 0));
            }

            if (!draft.LEntryDraftCollocated)
            {
                requests.Add(new LRequestCardAddition(id, LCardKind.LCardKindCollocation, 0, 0));
            }

            foreach (LCardDraft card in draft.LEntryDraftMeanings)
            {
                LEngineSentencePrepare(id, card, requests);
            }

            foreach (LCardDraft card in draft.LEntryDraftCollocations)
            {
                LEngineSentencePrepare(id, card, requests);
            }

            LGlyph? glyph = LEngineGlyphRead(draft.LEntryDraftLanguage);
            if (LEngineSchemeRead(draft.LEntryDraftLanguage) is [string scheme, ..]
                && !LGlyph.LGlyphOtherCheck(glyph, draft.LEntryDraftTranscriptions))
            {
                requests.Add(new LRequestTranscriptionAddition(id, scheme, 0, true));
            }

            if (glyph is not null && !LGlyph.LGlyphRowCheck(glyph, draft.LEntryDraftTranscriptions))
            {
                requests.Add(new LRequestTranscriptionAddition(
                    id, glyph.LGlyphName, draft.LEntryDraftTranscriptions.Count, true));
            }

            return requests;
        }
    }

    private static void LEngineSentencePrepare(long id, LCardDraft card, List<LRequest> requests)
    {
        if (!card.LCardDraftExemplified)
        {
            requests.Add(new LRequestSentenceAddition(id, card.LCardDraftId, 0));
        }
    }

    internal LDraft? LEngineDraftRead(long id)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            return LEngineCreditUpdate(_lEngineDrafts.LDraftRead(id));
        }
    }

    private LDraft? LEngineCreditUpdate(LDraft? draft)
    {
        if (draft is null || draft.LDraftAuthor.Count == 0)
        {
            return draft;
        }

        LAuthorVault archive = _lEngineAuthors;
        List<LAuthor> named = new(draft.LDraftAuthor.Count);
        foreach (LAuthor author in draft.LDraftAuthor)
        {
            named.Add(author.LAuthorStored ? archive.LAuthorRead(author.LAuthorId) ?? author : author);
        }

        return draft with { LDraftAuthor = named };
    }

    internal IReadOnlyList<LDraft> LEngineDraftScan()
    {
        lock (_lEngineGate)
        {
            return _lEngineDrafts.LDraftScan();
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
            _lEngineClaims.LClaimDelete(id);
            _lEngineDrafts.LDraftDelete(id);
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

            LDraft? draft = _lEngineDrafts.LDraftRead(id);
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

        if (draft.LDraftAuthorHeld is LAuthor author)
        {
            return LEngineAuthorCheck(draft, author);
        }

        LEntryDraft origin = draft.LDraftEntryId <= 0
            ? LEngineDraftBlank with { LEntryDraftLanguage = draft.LDraftContent.LEntryDraftLanguage }
            : LEngineEntryLoad(draft.LDraftEntryId) ?? LEngineDraftBlank;

        return !LEngineDraftMatch(origin, draft.LDraftContent);
    }

    internal void LEngineDraftSweep(long id)
    {
        LDraft saved;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            saved = LEngineDraftLoad(id).LDraftNormalize();
            _lEngineDrafts.LDraftSave(saved);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
    }

    internal LOutcome LEngineDraftCommit(long id)
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

            using (LVaultSession session = _lEngineVault.LVaultSessionStart())
            {
                outcome = LEngineDraftCommit(id, true, loaded, settled, deferred, finished, written);
                LEngineCourtApply(loaded, settled, deferred);
                session.LVaultSessionCommit();
            }

            foreach (Action step in written)
            {
                step();
            }

            foreach (long done in finished)
            {
                _lEngineDraftHeld.Remove(done);
                LEngineChronicleClear(done);
                _lEngineClaims.LClaimDelete(done);
                _lEngineDrafts.LDraftDelete(done);
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

            if (_lEngineDrafts.LDraftRead(link.LCourtTargetId) is null)
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
        written.Add(() => _lEngineDrafts.LDraftSave(saved));
        written.Add(() =>
        {
            foreach (LCourt link in _lEngineCourts.LCourtSettle(id))
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

            foreach (LCourt link in _lEngineCourts.LCourtSettle(id))
            {
                LEngineCourtUpdate(link, 0);
            }

            _lEngineDraftHeld.Remove(id);
            LEngineChronicleClear(id);
            _lEngineTrove.LTroveClear(id);
            _lEngineClaims.LClaimDelete(id);
            _lEngineDrafts.LDraftDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    private bool LEngineHoldCheck(long id)
    {
        LClaim? claim = _lEngineClaims.LClaimCheck(id)
            ? _lEngineClaims.LClaimRead(id)
            : null;

        return claim is not null
            && claim.LClaimProcess == Environment.ProcessId
            && _lEngineDraftHeld.Contains(id);
    }

    private bool LEngineClaimCheck(long id)
    {
        if (!_lEngineClaims.LClaimCheck(id))
        {
            return false;
        }

        LClaim? claim = _lEngineClaims.LClaimRead(id);
        return claim is not null && claim.LClaimProcess != Environment.ProcessId;
    }

    private static LEntryDraft LEngineDraftBlank =>
        new(string.Empty, string.Empty, null, string.Empty, [], []);

    private static string? LEngineRefusalRead(LDraft draft)
    {
        if (draft.LDraftExample is not null
            || draft.LDraftSituation is not null
            || draft.LDraftReference is not null
            || draft.LDraftAuthorHeld is not null)
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

        return _lEngineDrafts.LDraftRead(id)
            ?? throw new LRefusal(LRefusal.LRefusalDraft);
    }
}
