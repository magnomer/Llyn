using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntrySave(LEntryDraft draft)
    {
        return LEngineEntrySave(draft, []);
    }

    private LEntry LEngineEntrySave(LEntryDraft draft, Dictionary<long, long> identity)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(draft);

            if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
            {
                throw new LRefusal(LRefusal.LRefusalHeadword);
            }

            draft = _lEngineLanguageCache.LLanguageRespellingUpdate(LEngineDraftNormalize(draft) with
            {
                LEntryDraftHeadword = draft.LEntryDraftHeadword.Trim(),
            });

            using LVaultSession session = _lEngineVault.LVaultSessionStart();

            LEntry entry = _lEngineEntryClerk.LEntryClerkSave(draft, identity);
            LEngineTranscriptionSync(
                entry.LEntryId, LEngineTranscriptionReset(draft.LEntryDraftTranscriptions), null, identity);
            LEngineReflexSync(
                entry.LEntryId,
                draft.LEntryDraftLanguage,
                LEngineReflexReset(draft.LEntryDraftReflexes),
                null,
                identity);

            session.LVaultSessionCommit();
            LEngineFrequencyStart(entry.LEntryId);
            return entry;
        }
    }

    internal LEntry LEngineEntryUpdate(long id, LEntryDraft draft)
    {
        return LEngineEntryUpdate(id, draft, []);
    }

    private LEntry LEngineEntryUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            ArgumentNullException.ThrowIfNull(draft);

            using LVaultSession session = _lEngineVault.LVaultSessionStart();

            LEntry stored = _lEngineEntryClerk.LEntryClerkRead(id)
                ?? throw new LRefusal(LRefusal.LRefusalEntry);

            List<LRevisionChange> changes = [];
            LEntry updated = LEngineEntryUpdate(id, draft, identity, changes);

            if (changes.Count > 0)
            {
                _lEngineEntryClerk.LRevisionRecord(changes);
            }

            session.LVaultSessionCommit();
            bool renamed =
                !string.Equals(stored.LEntryHeadword, updated.LEntryHeadword, StringComparison.Ordinal) ||
                !string.Equals(stored.LEntryLanguage, updated.LEntryLanguage, StringComparison.Ordinal);
            if (renamed)
            {
                LEngineFrequencyStart(id);
            }

            return updated;
        }
    }

    private LEntry LEngineEntryUpdate(
        long id, LEntryDraft draft, Dictionary<long, long> identity, List<LRevisionChange> changes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(draft);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }

        draft = draft with { LEntryDraftHeadword = draft.LEntryDraftHeadword.Trim() };

        using LVaultSession session = _lEngineVault.LVaultSessionStart();

        LEntryDraft? origin = LEngineEntryLoad(id);
        bool changed = origin is null || !LEngineDraftMatch(origin, draft);

        LEngineInflectionReset(id);
        LEntry updated = _lEngineEntryClerk.LEntryClerkSave(id, draft, changed, identity, changes);
        LEngineTranscriptionSync(id, draft.LEntryDraftTranscriptions, changes, identity);
        LEngineReflexSync(id, draft.LEntryDraftLanguage, draft.LEntryDraftReflexes, changes, identity);

        session.LVaultSessionCommit();
        return updated;
    }

    private void LEngineRevisionRecord(long target, string subject, string kind, string? summary)
    {
        _lEngineEntryClerk.LRevisionRecord([new LRevisionChange(0, target, subject, kind, summary)]);
    }

    private void LEngineUpdatedSet(long entryId)
    {
        _lEngineEntryClerk.LEntryUpdatedSet(entryId);
    }

    private void LEngineUpdatedSet(long ownerId, bool collocation)
    {
        _lEngineCardClerk.LCardUpdatedSet(ownerId, collocation);
    }

    private static void LEngineFieldSync<LEngineRow, LEngineWritten>(
        IEnumerable<LEngineWritten> written,
        IReadOnlyList<LEngineRow> attached,
        Func<LEngineRow, long> identify,
        Func<LEngineWritten, long> resolve,
        Action<long> detach,
        Action<long, int> attach)
    {
        LCardClerkField.LCardFieldSync(written, attached, identify, resolve, detach, attach);
    }
}
