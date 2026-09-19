using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(entry);
            return _lEngineEntries.LEntryCreate(entry, forms, speeches);
        }
    }

    public LEntry? LEngineEntryRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntries.LEntryRead(id);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntries.LEntryFind(query);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return LCatalogEntry.LCatalogEntrySort(
                _lEngineEntries.LEntryFind(query),
                order);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(LEngineEntryFind(query, order), entry => entry.LEntryLanguage);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(tag);
            return _lEngineEntries.LEntryTagFind(tag.LTagId);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(LEngineEntryFind(tag), entry => entry.LEntryLanguage);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        return LEngineEntryMatch(LEngineEntryFind(tag, filter), query);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(register);
            return _lEngineEntries.LEntryRegisterFind(register.LRegisterId);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(LEngineEntryFind(register), entry => entry.LEntryLanguage);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        return LEngineEntryMatch(LEngineEntryFind(register, filter), query);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(situation);
            return _lEngineEntries.LEntrySituationFind(situation.LSituationId);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(filter);
        return LEngineEntryMatch(
            filter.LCatalogFilterApply(LEngineEntryFind(situation), entry => entry.LEntryLanguage),
            query);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(example);
            return _lEngineEntries.LEntryExampleFind(example.LExampleId);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(filter);
        return LEngineEntryMatch(
            filter.LCatalogFilterApply(LEngineEntryFind(example), entry => entry.LEntryLanguage),
            query);
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(reference);
            return _lEngineEntries.LEntryReferenceFind(reference.LReferenceId);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference, string query, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(filter);
        return LEngineEntryMatch(
            filter.LCatalogFilterApply(LEngineEntryFind(reference), entry => entry.LEntryLanguage),
            query);
    }

    private static IReadOnlyList<LEntry> LEngineEntryMatch(IReadOnlyList<LEntry> entries, string query)
    {
        string trimmed = query.Trim();
        return trimmed.Length == 0
            ? entries
            : [.. entries.Where(entry => LCatalog.LCatalogTextMatch(entry.LEntryHeadword, trimmed))];
    }

    public LEntryDraft? LEngineEntryLoad(long id)
    {
        lock (_lEngineGate)
        {
            LEntryDraft? draft = _lEngineEntries.LEntryLoad(id);
            if (draft is null)
            {
                return null;
            }

            List<LPronunciationDraft> resolved = new(draft.LEntryDraftPronunciations.Count);
            foreach (LPronunciationDraft spoken in draft.LEntryDraftPronunciations)
            {
                resolved.Add(spoken.LPronunciationDraftAudio.Length == 0
                    ? spoken
                    : spoken with
                    {
                        LPronunciationDraftAudio = LEngineRecordingResolve(spoken.LPronunciationDraftAudio),
                    });
            }

            return draft with { LEntryDraftPronunciations = resolved };
        }
    }

    private LEntryDraft LEngineRespellingUpdate(LEntryDraft draft)
    {
        List<LPronunciationDraft> spoken = new(draft.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft row in draft.LEntryDraftPronunciations)
        {
            spoken.Add(row.LPronunciationDraftRespelling.Length == 0
                ? LEngineRespellingResolve(draft.LEntryDraftLanguage, row)
                : row);
        }

        List<LReflexDraft> reflexes = new(draft.LEntryDraftReflexes.Count);
        foreach (LReflexDraft row in draft.LEntryDraftReflexes)
        {
            reflexes.Add(LEngineAnatomyResolve(
                draft.LEntryDraftLanguage,
                row.LReflexDraftRespelling.Length == 0 ? LEngineRespellingResolve(row) : row));
        }

        return draft with { LEntryDraftPronunciations = spoken, LEntryDraftReflexes = reflexes };
    }

    internal LRevision LEngineEntryDelete(long id)
    {
        LRevision recorded;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            using LVaultSession session = _lEngineVault.LVaultSessionStart();

            LEntry? deleted = _lEngineEntries.LEntryRead(id);
            _lEngineEntries.LEntryDelete(id);

            LRevisionChange change = new(0, id, "entry", "delete", deleted?.LEntryHeadword);
            LRevision revision = _lEngineRevisions.LRevisionRecord([change]);
            _lEngineTombstones.LTombstoneRecord(id, revision.LRevisionId);
            LWorkspaceState state = _lEngineWorkspaces.LWorkspaceStateRead();
            _lEngineWorkspaces.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

            session.LVaultSessionCommit();
            recorded = revision;
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, id);
        return recorded;
    }

    internal LTombstone? LEngineTombstoneRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineTombstones.LTombstoneRead(entryId);
        }
    }

    internal LRevision? LEngineRevisionRead()
    {
        lock (_lEngineGate)
        {
            long? id = _lEngineWorkspaces.LWorkspaceStateRead().LWorkspaceStateRevision;
            return id is null ? null : _lEngineRevisions.LRevisionRead(id.Value);
        }
    }

    internal IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)
    {
        lock (_lEngineGate)
        {
            return _lEngineRevisions.LRevisionChangeRead(revisionId);
        }
    }
}
