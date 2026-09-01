using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The entry half of the engine: an Entry created, read, searched for, loaded back into the draft the
/// input form saved, and deleted with the history that deletion leaves behind. It sits apart from the
/// composition root because it is one area of the model rather than a fact about the engine itself,
/// and because the delete is the shape every spanning operation here follows — several stores, one
/// session, one decision.
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Creates <paramref name="entry"/> with <paramref name="forms"/> and <paramref name="speeches"/>
    /// as its ordered child rows, and returns the stored entry with its assigned id and timestamps.
    /// </summary>
    public LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return new LEntryArchive(_lEngineDatabase).LEntryCreate(entry, forms, speeches);
    }

    /// <summary>Reads the entry for <paramref name="id"/>, or <c>null</c> when no entry has that id.</summary>
    public LEntry? LEngineEntryRead(string id)
    {
        return new LEntryArchive(_lEngineDatabase).LEntryRead(id);
    }

    /// <summary>
    /// Returns the entries whose headword contains <paramref name="query"/>, ordered by headword, or
    /// every entry when <paramref name="query"/> is empty or all whitespace — the list a browsing or
    /// searching pane shows. Matching is a contains whose case is folded over the whole of Unicode, so
    /// an accented headword is found typed in either case.
    /// </summary>
    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        return new LEntryArchive(_lEngineDatabase).LEntryFind(query);
    }

    /// <summary>
    /// Reads the entry identified by <paramref name="id"/> back into the draft the input form saved, or
    /// <c>null</c> when no entry has that id. This is the inverse of <see cref="LEngineEntrySave"/>: it
    /// composes the entry row, its meanings and collocations, its note and pronunciation, and the
    /// Examples, Situations and Tags each card references — all of them, in stored order — into one value
    /// the shell can put back on screen, read as a single consistent snapshot. A card's synonym comes back
    /// empty: no card writes one, and the links a Meaning or Collocation holds are read through the
    /// engine's relation seam instead, which returns targets rather than text.
    /// </summary>
    public LEntryDraft? LEngineEntryLoad(string id)
    {
        LEntryDraft? draft = new LEntryLoader(_lEngineDatabase).LEntryLoad(id);
        if (draft is null || draft.LEntryDraftAudio.Length == 0)
        {
            return draft;
        }

        // Stored relative, handed out full: the shell plays a file, so it never has to know the
        // workspace folder, and the same entry opened from a moved workspace still resolves.
        return draft with { LEntryDraftAudio = LEngineRecordingResolve(draft.LEntryDraftAudio) };
    }

    /// <summary>
    /// Deletes the entry identified by <paramref name="id"/> with everything it owns, then writes the
    /// history the deletion leaves behind: a revision carrying one delete change for the entry, a
    /// tombstone filed under that revision, and the workspace row moved onto it. Returns the recorded
    /// revision.
    /// <para>
    /// The delete runs first, so a refused delete — an entry another entry still links to — records no
    /// history at all and throws its own message through.
    /// </para>
    /// <para>
    /// All four writes share one session, so they are one transaction: the deleted entry, the revision
    /// recording it, the tombstone filed under that revision, and the workspace row moved onto it either
    /// all land or none of them do. Without it a failure part-way would leave an entry deleted with no
    /// tombstone naming it — history that no longer describes the file.
    /// </para>
    /// </summary>
    public LRevision LEngineEntryDelete(string id)
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

    /// <summary>
    /// Reads the tombstone left by deleting the Entry identified by <paramref name="entryId"/>, or
    /// <c>null</c> when that Entry has never been deleted.
    /// </summary>
    public LTombstone? LEngineTombstoneRead(string entryId)
    {
        return new LTombstoneArchive(_lEngineDatabase).LTombstoneRead(entryId);
    }

    /// <summary>Reads the most recently opened revision, or <c>null</c> when the workspace has none yet.</summary>
    public LRevision? LEngineRevisionRead()
    {
        return new LRevisionArchive(_lEngineDatabase).LRevisionLatestRead();
    }

    /// <summary>Reads the changes recorded under <paramref name="revisionId"/>, in the order recorded.</summary>
    public IReadOnlyList<LRevisionChange> LEngineChangeRead(string revisionId)
    {
        return new LRevisionArchive(_lEngineDatabase).LRevisionChangeRead(revisionId);
    }
}
