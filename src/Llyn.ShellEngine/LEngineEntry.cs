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

    public LEntry? LEngineEntryRead(string id)
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

    public LEntryDraft? LEngineEntryLoad(string id)
    {
        lock (_lEngineGate)
        {
            LEntryDraft? draft = new LEntryLoader(_lEngineDatabase).LEntryLoad(id);
            if (draft is null || draft.LEntryDraftAudio.Length == 0)
            {
                return draft;
            }

            return draft with { LEntryDraftAudio = LEngineRecordingResolve(draft.LEntryDraftAudio) };
        }
    }

    public LRevision LEngineEntryDelete(string id)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

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
            return revision;
        }
    }

    public LTombstone? LEngineTombstoneRead(string entryId)
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
            return new LRevisionArchive(_lEngineDatabase).LRevisionLatestRead();
        }
    }

    public IReadOnlyList<LRevisionChange> LEngineChangeRead(string revisionId)
    {
        lock (_lEngineGate)
        {
            return new LRevisionArchive(_lEngineDatabase).LRevisionChangeRead(revisionId);
        }
    }
}
