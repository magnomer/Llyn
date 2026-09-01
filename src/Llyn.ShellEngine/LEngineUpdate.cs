using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The one write path that changes a stored entry: the same draft the save consumes goes in, and the
/// rows the entry is already made of are reconciled to it. It is the counterpart of
/// <c>LEngineEntrySave</c> — the save creates, this one changes — and it lives beside it rather than
/// inside it because the two share nothing but the draft shape: creating writes rows in card order,
/// changing has to work out which stored row each card is first.
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Applies <paramref name="draft"/> to the entry identified by <paramref name="id"/> and returns
    /// the stored entry as it now stands. The entry keeps its opaque id and its <c>added_utc</c>; only
    /// <c>updated_utc</c> moves, so an edit is the same record, not a new one.
    /// <para>
    /// A blank headword and an id no entry carries are both refused before anything is written, each
    /// with a reason key the shell localizes. The refusal for a missing entry matters: the form may
    /// have been opened on an entry that has since been deleted, and falling back to creating a copy
    /// is exactly the defect this seam exists to remove.
    /// </para>
    /// <para>
    /// Everything after the refusals runs inside one session and commits once, so an update is whole
    /// or it never happened: a card that cannot be deleted — one another entry still links to — leaves
    /// the stored entry exactly as it was, down to its timestamps, rather than half-applied.
    /// </para>
    /// <para>
    /// Which stored card a draft card is comes from <see cref="LCardDraft.LCardDraftId"/>, never from
    /// its place in the list: a card naming a stored Meaning or Collocation of this entry updates that
    /// row in place, so its id survives and every row referencing it keeps pointing at the same
    /// Meaning. A card naming nothing is created, a stored card the draft no longer names is deleted,
    /// and a card gone entirely blank counts as dropped on the same terms the save counts it as
    /// unwritten. The survivors are then renumbered to draft order in one pass through
    /// <see cref="LDatabaseOrder"/>, because the unique <c>(owner, position)</c> index makes moving one
    /// row at a time collide on the first statement.
    /// </para>
    /// <para>
    /// A card's Examples, Situations and Tags are re-attached to match the draft: a value the card
    /// still lists keeps the row it already referenced, a new value creates a new row, and a value it
    /// dropped is detached and nothing more. Detaching the last reference never deletes the row —
    /// those are independent data the card references and does not own, so a Tag typed once and
    /// cleared survives its last referrer.
    /// </para>
    /// <para>
    /// The revision records one change per altered child — each Meaning and Collocation created,
    /// updated or deleted, and the note and pronunciation when they moved — rather than one change
    /// saying the entry changed, so the history says what an edit actually did. The workspace row is
    /// moved onto that revision, as the save and the delete both do.
    /// </para>
    /// </summary>
    public LEntry LEngineEntryUpdate(string id, LEntryDraft draft)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(draft);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEntryArchive entries = new(_lEngineDatabase);
        LEntry stored = entries.LEntryRead(id) ?? throw new LRefusal(LRefusal.LRefusalEntry);

        List<LRevisionChange> changes = [];

        // The row is updated in place: the archive stamps updated_utc and touches neither the id nor
        // added_utc, so the entry stays the record it was.
        entries.LEntryUpdate(stored with
        {
            LEntryHeadword = draft.LEntryDraftHeadword,
            LEntryLanguage = draft.LEntryDraftLanguage,
        });

        if (!string.Equals(stored.LEntryHeadword, draft.LEntryDraftHeadword, StringComparison.Ordinal) ||
            !string.Equals(stored.LEntryLanguage, draft.LEntryDraftLanguage, StringComparison.Ordinal))
        {
            changes.Add(new LRevisionChange(0, id, "entry", "update", draft.LEntryDraftHeadword));
        }

        LEngineCardUpdate(
            session.LDatabaseSessionConnection,
            id,
            draft.LEntryDraftSenses,
            draft.LEntryDraftLanguage,
            collocation: false,
            changes);
        LEngineCardUpdate(
            session.LDatabaseSessionConnection,
            id,
            draft.LEntryDraftCollocations,
            draft.LEntryDraftLanguage,
            collocation: true,
            changes);

        LEngineNoteUpdate(id, draft, changes);
        LEnginePronunciationUpdate(id, draft, changes);

        LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord(changes);

        LWorkspaceArchive workspace = new(_lEngineDatabase);
        LWorkspaceState state = workspace.LWorkspaceStateRead();
        workspace.LWorkspaceStateSave(state with
        {
            LWorkspaceStateLeft = id,
            LWorkspaceStateRevision = revision.LRevisionId,
        });

        LEntry updated = entries.LEntryRead(id) ?? stored;
        session.LDatabaseSessionCommit();
        return updated;
    }

    // The entry's note reconciled to the draft: text replaces whatever was stored, and a note the user
    // cleared is deleted rather than left standing as the last thing they typed.
    private void LEngineNoteUpdate(string entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LNoteArchive notes = new(_lEngineDatabase);
        LNote? stored = notes.LNoteRead(entryId);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftNote))
        {
            if (stored is not null)
            {
                notes.LNoteDelete(entryId);
                changes.Add(new LRevisionChange(0, entryId, "note", "delete", null));
            }

            return;
        }

        if (string.Equals(stored?.LNoteText, draft.LEntryDraftNote, StringComparison.Ordinal))
        {
            return;
        }

        notes.LNoteSave(new LNote(entryId, draft.LEntryDraftNote));
        changes.Add(new LRevisionChange(
            0, entryId, "note", stored is null ? "create" : "update", null));
    }

    // The entry's pronunciation reconciled to the draft. The row is what a recording hangs from, so it
    // is created when either the IPA or a recording is present and deleted only when both are gone —
    // and its id survives an edit of the IPA, so the recording hanging from it is not re-downloaded to
    // stay attached.
    private void LEnginePronunciationUpdate(
        string entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LPronunciationArchive pronunciations = new(_lEngineDatabase);
        LPronunciation? stored = pronunciations.LPronunciationRead(entryId);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftPronunciation) &&
            string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
        {
            if (stored is not null)
            {
                pronunciations.LPronunciationDelete(stored.LPronunciationId);
                changes.Add(new LRevisionChange(
                    0, stored.LPronunciationId, "pronunciation", "delete", stored.LPronunciationIpa));
            }

            return;
        }

        string pronunciationId;
        if (stored is null)
        {
            LPronunciation created = pronunciations.LPronunciationCreate(new LPronunciation(
                string.Empty, entryId, null, draft.LEntryDraftPronunciation, [], []));
            pronunciationId = created.LPronunciationId;
            changes.Add(new LRevisionChange(
                0, pronunciationId, "pronunciation", "create", draft.LEntryDraftPronunciation));
        }
        else
        {
            pronunciationId = stored.LPronunciationId;
            if (!string.Equals(
                    stored.LPronunciationIpa, draft.LEntryDraftPronunciation, StringComparison.Ordinal))
            {
                pronunciations.LPronunciationUpdate(stored with
                {
                    LPronunciationIpa = draft.LEntryDraftPronunciation,
                });
                changes.Add(new LRevisionChange(
                    0, pronunciationId, "pronunciation", "update", draft.LEntryDraftPronunciation));
            }
        }

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
        {
            return;
        }

        // The draft carries the recording as a full path and the row stores it relative to the
        // workspace, so the comparison is made in stored terms; an unchanged recording writes nothing.
        string file = LEngineRecordingFormat(draft.LEntryDraftAudio);
        LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciationId);
        if (string.Equals(audio?.LPronunciationAudioFile, file, StringComparison.Ordinal))
        {
            return;
        }

        pronunciations.LPronunciationAudioSave(pronunciationId, file, draft.LEntryDraftSource);
        changes.Add(new LRevisionChange(0, pronunciationId, "pronunciation", "update", file));
    }
}
