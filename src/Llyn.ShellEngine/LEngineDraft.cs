using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LEntry LEngineEntrySave(LEntryDraft draft)
    {
        lock (_lEngineGate)
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

            LMeaningArchive meanings = new(_lEngineDatabase);
            foreach (LCardDraft card in LEngineCardRead(draft.LEntryDraftMeanings))
            {
                LMeaning meaning = meanings.LMeaningCreate(new LMeaning(
                    string.Empty,
                    entry.LEntryId,
                    null,
                    0,
                    card.LCardDraftTitle,
                    null,
                    null,
                    card.LCardDraftMeaning,
                    string.Empty));

                LEngineCardAttach(meaning.LMeaningId, card, draft.LEntryDraftLanguage, collocation: false);
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
            workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

            session.LDatabaseSessionCommit();
            return entry;
        }
    }

    private void LEngineCardAttach(string ownerId, LCardDraft card, string language, bool collocation)
    {
        LEngineSentenceSync(ownerId, card.LCardDraftExample, language, collocation);

        int position;

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
                situations.LSituationMeaningAttach(ownerId, situationId, position);
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
                images.LImageMeaningAttach(ownerId, image.LImageId, position);
            }

            position++;
        }

        LVideoArchive videos = new(_lEngineDatabase);
        position = 0;
        foreach (LStateValue location in LEngineFieldRead(card.LCardDraftVideo))
        {
            LVideo video = videos.LVideoCreate(new LVideo(string.Empty, location));
            if (collocation)
            {
                videos.LVideoCollocationAttach(ownerId, video.LVideoId, position);
            }
            else
            {
                videos.LVideoMeaningAttach(ownerId, video.LVideoId, position);
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
                LEngineFieldCheck(card.LCardDraftImage) ||
                LEngineFieldCheck(card.LCardDraftVideo))
            {
                yield return card;
            }
        }
    }

    private static bool LEngineExampleCheck(IReadOnlyList<LExampleDraft> drafts)
    {
        foreach (LExampleDraft draft in drafts)
        {
            if (LEngineExampleCheck(draft))
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
            if (LEngineExampleCheck(draft))
            {
                yield return draft;
            }
        }
    }

    private static bool LEngineExampleCheck(LExampleDraft draft)
    {
        return !draft.LExampleDraftText.LStateValueEmpty
            || !draft.LExampleDraftParticle.LStateValueEmpty
            || !draft.LExampleDraftDependence.LStateValueEmpty;
    }

    private static LExample? LEngineExampleResolve(
        LExampleArchive examples, LExampleDraft draft, string language)
    {
        if (draft.LExampleDraftText.LStateValueEmpty)
        {
            return null;
        }

        return LEngineExampleResolve(
            examples,
            draft.LExampleDraftId,
            draft.LExampleDraftText,
            draft.LExampleDraftReference,
            language);
    }

    private static LExample LEngineExampleResolve(
        LExampleArchive examples,
        string id,
        LStateValue text,
        LStateValue reference,
        string language)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            LExample? stored = examples.LExampleRead(id);
            if (stored is not null)
            {
                if (stored.LExampleText != text)
                {
                    examples.LExampleTextUpdate(stored.LExampleId, text);
                    stored = stored with { LExampleText = text };
                }

                if (stored.LExampleSource != reference)
                {
                    examples.LExampleSourceUpdate(stored.LExampleId, reference);
                    stored = stored with { LExampleSource = reference };
                }

                return stored;
            }
        }

        return examples.LExampleCreate(new LExample(
            id, language, text, LStateValue.LStateValueUnspecified, reference));
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

                return stored.LSituationId;
            }
        }

        return situations.LSituationCreate(new LSituation(
            draft.LSituationDraftId,
            draft.LSituationDraftText,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified)).LSituationId;
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
