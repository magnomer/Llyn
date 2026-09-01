using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LEntry LEngineEntrySave(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

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
            speeches: LEngineSpeechResolve(
                string.Empty, draft.LEntryDraftLanguage, draft.LEntryDraftSpeech));

        LSenseArchive senses = new(_lEngineDatabase);
        foreach (LCardDraft card in LEngineCardRead(draft.LEntryDraftSenses))
        {
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
