using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The one write path for an input-form draft: the whole form goes in as a single value and comes out
/// as the rows one entry is made of. It is the inverse of <see cref="LEntryLoader"/>, so it lives on
/// its own rather than among the engine's lookup and workspace calls, and everything it writes shares
/// one session — an entry is written whole or not at all.
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Saves the whole input form as one new entry: the headword row, its senses and collocations in
    /// card order, its note and pronunciation when they carry text, the revision recording the create,
    /// and the workspace row moved onto that revision and that entry. Returns the stored entry with its
    /// assigned id and timestamps.
    /// <para>
    /// A blank headword is refused before any connection opens, so a save that cannot be made costs
    /// nothing. Everything after that runs inside one session, which is what makes a half-written entry
    /// impossible: a failure at any write rolls back every write before it, leaving no entry row behind.
    /// That is also why this is a single engine call — the shell has no way to compose a partial write
    /// out of several of them.
    /// </para>
    /// <para>
    /// A card with every field blank writes nothing. The form keeps an empty card on screen to type
    /// into and refuses to remove a list's last card, so it always hands over at least one Meaning card
    /// and one Collocation card; deciding that a blank one is neither belongs here, not in the form. A
    /// headword saved on its own therefore gains no sense row and no collocation row, and a blank card
    /// between two filled ones leaves the two stored at positions 0 and 1.
    /// </para>
    /// <para>
    /// A recording the downloader saved is written as the pronunciation's audio row, in the same
    /// transaction, with its path made relative to the workspace so a moved workspace keeps its audio.
    /// Because the row hangs off the pronunciation, a recording with no typed IPA still creates the
    /// pronunciation to hang from.
    /// </para>
    /// <para>
    /// The Example, Situation and Tag text a card carries is written as independent data: each value in
    /// those ordered sets becomes a new row of its own entity, which the card's sense or collocation then
    /// references at that value's position. The entry owns none of them, so clearing a card would only
    /// detach what it points at.
    /// </para>
    /// <para>
    /// Text is never matched against an existing Example, Situation or Tag: every non-empty value
    /// creates a new row, even when the same words were saved before. Matching needs a picker that
    /// resolves typed text to a chosen row, and none exists yet.
    /// </para>
    /// <para>
    /// Parts of speech are not written: the input panel has no control to choose one with. The
    /// vocabulary itself is there — <c>part_of_speech_value</c> is written from the language packs when
    /// the engine binds to a workspace — so what is missing is the control, not the data behind it. <b>Synonyms are not written either</b>, and no card offers one to type. An
    /// <c>LSynonym</c> targets an Entry or a Meaning by id, and a sense-card synonym is an
    /// <c>LRelation</c> with the same requirement; free text is neither, and no picker exists to resolve
    /// it, so the field was removed from the form rather than left to discard what was typed.
    /// <c>LCardDraftSynonym</c> therefore arrives empty from both card kinds. Writing a link is its own
    /// seam — <c>LEngineRelationCreate</c> and <c>LEngineSynonymCreate</c>, each taking a target the
    /// caller resolved through <c>LEngineEntryFind</c> or <c>LEngineSenseFind</c> first.
    /// </para>
    /// </summary>
    public LEntry LEngineEntrySave(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            // The refusal names its reason with a key; the shell turns that into localized text, so no
            // user-facing wording lives in the engine.
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        // The entry's language is the language-pack name, which is the key part_of_speech_value and
        // morphology_value are already written against.
        LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(
            new LEntry(
                string.Empty,
                draft.LEntryDraftHeadword,
                draft.LEntryDraftLanguage,
                null,
                null,
                null,
                null),
            forms: [],
            speeches: []);

        LSenseArchive senses = new(_lEngineDatabase);
        foreach (LCardDraft card in LEngineCardRead(draft.LEntryDraftSenses))
        {
            // Each sense is appended, so card order becomes stored position.
            LSense sense = senses.LSenseCreate(new LSense(
                string.Empty,
                entry.LEntryId,
                null,
                0,
                card.LCardDraftTitle,
                null,
                null,
                card.LCardDraftMeaning,
                string.Empty));

            LEngineCardAttach(sense.LSenseId, card, draft.LEntryDraftLanguage, collocation: false);
        }

        LCollocationArchive collocations = new(_lEngineDatabase);
        foreach (LCardDraft card in LEngineCardRead(draft.LEntryDraftCollocations))
        {
            LCollocation collocation = collocations.LCollocationCreate(new LCollocation(
                string.Empty,
                entry.LEntryId,
                0,
                card.LCardDraftTitle,
                card.LCardDraftExpression,
                card.LCardDraftMeaning));

            LEngineCardAttach(
                collocation.LCollocationId, card, draft.LEntryDraftLanguage, collocation: true);
        }

        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftNote))
        {
            new LNoteArchive(_lEngineDatabase).LNoteSave(new LNote(entry.LEntryId, draft.LEntryDraftNote));
        }

        // The pronunciation row is what a recording hangs from, so a downloaded recording creates one
        // even when no IPA was typed; without it the audio would have nothing to reference.
        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftPronunciation) ||
            !string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
        {
            LPronunciationArchive pronunciations = new(_lEngineDatabase);
            LPronunciation pronunciation = pronunciations.LPronunciationCreate(new LPronunciation(
                string.Empty,
                entry.LEntryId,
                null,
                draft.LEntryDraftPronunciation,
                [],
                []));

            if (!string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
            {
                pronunciations.LPronunciationAudioSave(
                    pronunciation.LPronunciationId,
                    LEngineRecordingFormat(draft.LEntryDraftAudio),
                    draft.LEntryDraftSource);
            }
        }

        LRevisionChange change = new(0, entry.LEntryId, "entry", "create", entry.LEntryHeadword);
        LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);

        LWorkspaceArchive workspace = new(_lEngineDatabase);
        LWorkspaceState state = workspace.LWorkspaceStateRead();
        workspace.LWorkspaceStateSave(state with
        {
            LWorkspaceStateLeft = entry.LEntryId,
            LWorkspaceStateRevision = revision.LRevisionId,
        });

        session.LDatabaseSessionCommit();
        return entry;
    }

    /// <summary>
    /// Writes the Example, Situation and Tag a card typed and points the stored card at them. Each
    /// non-empty field creates a row of its own — independent data the card references rather than
    /// owns — and an empty field writes nothing at all.
    /// <para>
    /// A Meaning card and a Collocation card carry the same three fields on the same terms, so this is
    /// the one write path for both: <paramref name="collocation"/> says which of the two owner sides
    /// <paramref name="ownerId"/> names, and nothing else about the two differs.
    /// </para>
    /// <para>
    /// Each field is an ordered set: every value the card lists becomes a row of its own, referenced at
    /// the position it holds in that list, so three tags are three rows at 0, 1 and 2 and read back in
    /// the order they were typed. The list arrives as the shell built it — the engine never splits text
    /// into rows. A card that lists nothing for a field attaches nothing, which detaches the field
    /// rather than deleting anything: the rows a card references are independent data it does not own.
    /// The card's own order is already carried by the row it produced.
    /// </para>
    /// </summary>
    private void LEngineCardAttach(string ownerId, LCardDraft card, string language, bool collocation)
    {
        LExampleArchive exampleRows = new(_lEngineDatabase);
        LExampleLink examples = new(_lEngineDatabase);
        int position = 0;
        foreach (string text in LEngineFieldRead(card.LCardDraftExample))
        {
            LExample example = exampleRows.LExampleCreate(
                new LExample(string.Empty, language, text, null, null, []));
            if (collocation)
            {
                examples.LExampleCollocationAttach(ownerId, example.LExampleId, position);
            }
            else
            {
                examples.LExampleSenseAttach(ownerId, example.LExampleId, position);
            }

            position++;
        }

        LSituationArchive situations = new(_lEngineDatabase);
        position = 0;
        foreach (string text in LEngineFieldRead(card.LCardDraftSituation))
        {
            LSituation situation = situations.LSituationCreate(
                new LSituation(string.Empty, text, null, null));
            if (collocation)
            {
                situations.LSituationCollocationAttach(ownerId, situation.LSituationId, position);
            }
            else
            {
                situations.LSituationSenseAttach(ownerId, situation.LSituationId, position);
            }

            position++;
        }

        LTagArchive tags = new(_lEngineDatabase);
        position = 0;
        foreach (string text in LEngineFieldRead(card.LCardDraftTag))
        {
            LTag tag = tags.LTagCreate(new LTag(string.Empty, text));
            if (collocation)
            {
                tags.LTagCollocationAttach(ownerId, tag.LTagId, position);
            }
            else
            {
                tags.LTagSenseAttach(ownerId, tag.LTagId, position);
            }

            position++;
        }
    }

    // The cards worth a row: a card with every field blank is neither a Meaning nor a Collocation, so
    // it is skipped rather than written. The form always hands over at least one card of each kind —
    // it seeds one of each and refuses to remove a list's last card, because an editor must keep an
    // empty card to type into — so this is where an entry with nothing typed stops becoming a sense
    // row with no definition and a collocation row with no expression. Positions come from the stored
    // sibling count, so skipping a card in the middle still leaves 0, 1, 2 over the cards that remain.
    private static IEnumerable<LCardDraft> LEngineCardRead(IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            if (!string.IsNullOrWhiteSpace(card.LCardDraftTitle) ||
                !string.IsNullOrWhiteSpace(card.LCardDraftExpression) ||
                !string.IsNullOrWhiteSpace(card.LCardDraftMeaning) ||
                !string.IsNullOrWhiteSpace(card.LCardDraftSynonym) ||
                LEngineFieldCheck(card.LCardDraftExample) ||
                LEngineFieldCheck(card.LCardDraftSituation) ||
                LEngineFieldCheck(card.LCardDraftTag))
            {
                yield return card;
            }
        }
    }

    // Whether one card field carries any value worth a row, on the same terms LEngineFieldRead writes
    // them: a field holding only blanks counts as typed-in nothing.
    private static bool LEngineFieldCheck(IReadOnlyList<string> texts)
    {
        foreach (string text in texts)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                return true;
            }
        }

        return false;
    }

    // The values of one card field that are worth a row: blank entries are dropped here rather than
    // written, so a list the shell built out of a control cannot leave an empty row behind, and the
    // positions stay a gapless 0, 1, 2 over what is actually stored.
    private static IEnumerable<string> LEngineFieldRead(IReadOnlyList<string> texts)
    {
        foreach (string text in texts)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                yield return text;
            }
        }
    }
}
