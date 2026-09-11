using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(entry);
            return new LEntryArchive(_lEngineDatabase).LEntryCreate(entry, forms, speeches);
        }
    }

    public LEntry? LEngineEntryRead(long id)
    {
        lock (_lEngineGate)
        {
            return new LEntryArchive(_lEngineDatabase).LEntryRead(id);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        lock (_lEngineGate)
        {
            return new LEntryArchive(_lEngineDatabase).LEntryFind(query);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return LCatalogEntry.LCatalogEntrySort(
                new LEntryArchive(_lEngineDatabase).LEntryFind(query),
                order);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(tag);
            return new LEntryArchive(_lEngineDatabase).LEntryTagFind(tag.LTagText);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(register);
            return new LEntryArchive(_lEngineDatabase).LEntryRegisterFind(register.LRegisterId);
        }
    }

    public LEntryDraft? LEngineEntryLoad(long id)
    {
        lock (_lEngineGate)
        {
            LEntryDraft? draft = new LEntryLoader(_lEngineDatabase).LEntryLoad(id);
            if (draft is null
                || draft.LEntryDraftPronunciation is not LPronunciationDraft spoken
                || spoken.LPronunciationDraftAudio.Length == 0)
            {
                return draft;
            }

            return draft with
            {
                LEntryDraftPronunciation = spoken with
                {
                    LPronunciationDraftAudio =
                        LEngineRecordingResolve(spoken.LPronunciationDraftAudio),
                },
            };
        }
    }

    public LRevision LEngineEntryDelete(long id)
    {
        LRevision recorded;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEntryArchive entries = new(_lEngineDatabase);
            LEntry? deleted = entries.LEntryRead(id);
            entries.LEntryDelete(id);

            LRevisionChange change = new(0, id, "entry", "delete", deleted?.LEntryHeadword);
            LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);
            new LTombstoneArchive(_lEngineDatabase).LTombstoneRecord(id, revision.LRevisionId);

            LWorkspaceArchive workspace = new(_lEngineDatabase);
            LWorkspaceState state = workspace.LWorkspaceStateRead();
            workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

            session.LDatabaseSessionCommit();
            recorded = revision;
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, id);
        return recorded;
    }

    public LTombstone? LEngineTombstoneRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return new LTombstoneArchive(_lEngineDatabase).LTombstoneRead(entryId);
        }
    }

    public LRevision? LEngineRevisionRead()
    {
        lock (_lEngineGate)
        {
            long? id = new LWorkspaceArchive(_lEngineDatabase).LWorkspaceStateRead().LWorkspaceStateRevision;
            return id is null ? null : new LRevisionArchive(_lEngineDatabase).LRevisionRead(id.Value);
        }
    }

    public IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)
    {
        lock (_lEngineGate)
        {
            return new LRevisionArchive(_lEngineDatabase).LRevisionChangeRead(revisionId);
        }
    }
}
