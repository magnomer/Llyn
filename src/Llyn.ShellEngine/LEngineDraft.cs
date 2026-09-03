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
                string.Empty, draft.LEntryDraftLanguage, draft.LEntryDraftSpeeches));

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
        HashSet<string> attachedExamples = new(StringComparer.Ordinal);
        foreach (LExampleDraft draft in LEngineExampleRead(card.LCardDraftExample))
        {
            string exampleId = LEngineExampleResolve(exampleRows, draft, language);
            if (!attachedExamples.Add(exampleId))
            {
                continue;
            }

            if (collocation)
            {
                examples.LExampleCollocationAttach(ownerId, exampleId, position);
            }
            else
            {
                examples.LExampleSenseAttach(ownerId, exampleId, position);
            }

            position++;
        }

        LSituationArchive situations = new(_lEngineDatabase);
        position = 0;
        HashSet<string> attachedSituations = new(StringComparer.Ordinal);
        foreach (LSituationDraft draft in LEngineSituationRead(card.LCardDraftSituation))
        {
            string situationId = LEngineSituationResolve(situations, draft);
            if (!attachedSituations.Add(situationId))
            {
                continue;
            }

            if (collocation)
            {
                situations.LSituationCollocationAttach(ownerId, situationId, position);
            }
            else
            {
                situations.LSituationSenseAttach(ownerId, situationId, position);
            }

            position++;
        }

        LEngineTagSave(ownerId, card.LCardDraftTag, collocation);
        LEngineTranslationSave(ownerId, card.LCardDraftTranslation, collocation);

        LImageArchive images = new(_lEngineDatabase);
        position = 0;
        foreach (LStateValue location in LEngineFieldRead(card.LCardDraftImage))
        {
            LImage image = images.LImageCreate(new LImage(string.Empty, location));
            if (collocation)
            {
                images.LImageCollocationAttach(ownerId, image.LImageId, position);
            }
            else
            {
                images.LImageSenseAttach(ownerId, image.LImageId, position);
            }

            position++;
        }
    }

    private static IEnumerable<LCardDraft> LEngineCardRead(IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            if (!card.LCardDraftTitle.LStateValueEmpty ||
                !card.LCardDraftExpression.LStateValueEmpty ||
                !card.LCardDraftMeaning.LStateValueEmpty ||
                !string.IsNullOrWhiteSpace(card.LCardDraftSynonym) ||
                LEngineExampleCheck(card.LCardDraftExample) ||
                LEngineSituationCheck(card.LCardDraftSituation) ||
                LEngineTagCheck(card.LCardDraftTag) ||
                LEngineTranslationCheck(card.LCardDraftTranslation) ||
                LEngineFieldCheck(card.LCardDraftImage))
            {
                yield return card;
            }
        }
    }

    private static bool LEngineExampleCheck(IReadOnlyList<LExampleDraft> drafts)
    {
        foreach (LExampleDraft draft in drafts)
        {
            if (!draft.LExampleDraftText.LStateValueEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<LExampleDraft> LEngineExampleRead(IReadOnlyList<LExampleDraft> drafts)
    {
        foreach (LExampleDraft draft in drafts)
        {
            if (!draft.LExampleDraftText.LStateValueEmpty)
            {
                yield return draft;
            }
        }
    }

    private static string LEngineExampleResolve(
        LExampleArchive examples, LExampleDraft draft, string language)
    {
        if (!string.IsNullOrWhiteSpace(draft.LExampleDraftId))
        {
            LExample? stored = examples.LExampleRead(draft.LExampleDraftId);
            if (stored is not null)
            {
                if (stored.LExampleText != draft.LExampleDraftText)
                {
                    examples.LExampleTextUpdate(stored.LExampleId, draft.LExampleDraftText);
                }

                if (stored.LExampleSource != draft.LExampleDraftReference)
                {
                    examples.LExampleSourceUpdate(stored.LExampleId, draft.LExampleDraftReference);
                }

                return stored.LExampleId;
            }
        }

        return examples.LExampleCreate(new LExample(
            draft.LExampleDraftId,
            language,
            draft.LExampleDraftText,
            null,
            draft.LExampleDraftReference,
            [])).LExampleId;
    }

    private static bool LEngineSituationCheck(IReadOnlyList<LSituationDraft> drafts)
    {
        foreach (LSituationDraft draft in drafts)
        {
            if (!draft.LSituationDraftText.LStateValueEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<LSituationDraft> LEngineSituationRead(IReadOnlyList<LSituationDraft> drafts)
    {
        foreach (LSituationDraft draft in drafts)
        {
            if (!draft.LSituationDraftText.LStateValueEmpty)
            {
                yield return draft;
            }
        }
    }

    private static string LEngineSituationResolve(LSituationArchive situations, LSituationDraft draft)
    {
        if (!string.IsNullOrWhiteSpace(draft.LSituationDraftId))
        {
            LSituation? stored = situations.LSituationRead(draft.LSituationDraftId);
            if (stored is not null)
            {
                if (stored.LSituationTitle != draft.LSituationDraftText)
                {
                    situations.LSituationTitleUpdate(stored.LSituationId, draft.LSituationDraftText);
                }

                if (stored.LSituationSource != draft.LSituationDraftReference)
                {
                    situations.LSituationSourceUpdate(stored.LSituationId, draft.LSituationDraftReference);
                }

                return stored.LSituationId;
            }
        }

        return situations.LSituationCreate(new LSituation(
            draft.LSituationDraftId,
            draft.LSituationDraftText,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            draft.LSituationDraftReference)).LSituationId;
    }

    private static bool LEngineTagCheck(IReadOnlyList<string> texts)
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

    private static bool LEngineTranslationCheck(IReadOnlyList<string> ids)
    {
        foreach (string id in ids)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return true;
            }
        }

        return false;
    }

    private static bool LEngineFieldCheck(IReadOnlyList<LStateValue> texts)
    {
        foreach (LStateValue text in texts)
        {
            if (!text.LStateValueEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<LStateValue> LEngineFieldRead(IReadOnlyList<LStateValue> texts)
    {
        foreach (LStateValue text in texts)
        {
            if (!text.LStateValueEmpty)
            {
                yield return text;
            }
        }
    }
}
