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
                forms: draft.LEntryDraftForms,
                speeches: LEngineSpeechResolve(
                    string.Empty, draft.LEntryDraftLanguage, draft.LEntryDraftSpeeches));

            if (draft.LEntryDraftInflections.Count > 0)
            {
                new LInflectionArchive(_lEngineDatabase).LInflectionSet(
                    entry.LEntryId, draft.LEntryDraftInflections);
            }

            LEngineCardValidate(draft.LEntryDraftMeanings, collocation: false);
            LEngineCardValidate(draft.LEntryDraftCollocations, collocation: true);

            foreach (LCardDraft card in LEngineCardRead(draft.LEntryDraftMeanings))
            {
                LEngineMeaningCreate(entry.LEntryId, null, card, draft.LEntryDraftLanguage);
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

            if (draft.LEntryDraftPronunciation is LPronunciationDraft spoken
                && !spoken.LPronunciationDraftEmpty)
            {
                LPronunciationArchive pronunciations = new(_lEngineDatabase);
                LPronunciation pronunciation = pronunciations.LPronunciationCreate(new LPronunciation(
                    string.Empty,
                    entry.LEntryId,
                    spoken.LPronunciationDraftLevel,
                    spoken.LPronunciationDraftIpa,
                    spoken.LPronunciationDraftSyllables,
                    spoken.LPronunciationDraftRepresentations));

                if (spoken.LPronunciationDraftAudio.Length > 0)
                {
                    pronunciations.LPronunciationAudioSave(
                        pronunciation.LPronunciationId,
                        LEngineRecordingFormat(spoken.LPronunciationDraftAudio),
                        spoken.LPronunciationDraftSource);
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

    private void LEngineMeaningCreate(
        string entryId, string? parentId, LCardDraft card, string language)
    {
        LMeaning meaning = new LMeaningArchive(_lEngineDatabase).LMeaningCreate(new LMeaning(
            string.Empty,
            entryId,
            parentId,
            0,
            card.LCardDraftTitle,
            card.LCardDraftGloss,
            card.LCardDraftLanguage,
            card.LCardDraftMeaning,
            card.LCardDraftLabels));

        LEngineCardAttach(meaning.LMeaningId, card, language, collocation: false);

        foreach (LCardDraft child in LEngineCardRead(card.LCardDraftChild))
        {
            LEngineMeaningCreate(entryId, meaning.LMeaningId, child, language);
        }
    }

    private static void LEngineCardValidate(IReadOnlyList<LCardDraft> cards, bool collocation)
    {
        if (!collocation)
        {
            return;
        }

        foreach (LCardDraft card in cards)
        {
            if (card.LCardDraftChild.Count > 0)
            {
                throw new LRefusal(LRefusal.LRefusalCollocation);
            }
        }
    }

    private void LEngineCardAttach(string ownerId, LCardDraft card, string language, bool collocation)
    {
        LEngineSentenceSync(ownerId, card.LCardDraftSentence, language, collocation);

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

        LEngineRegisterAttach(ownerId, card.LCardDraftRegister, language, collocation);

        LEngineTagSave(ownerId, card.LCardDraftTag, collocation);
        LEngineTranslationSave(ownerId, card.LCardDraftTranslation, collocation);

        LImageArchive images = new(_lEngineDatabase);
        position = 0;
        HashSet<string> attachedImages = new(StringComparer.Ordinal);
        foreach (LImageDraft draft in LEngineImageRead(card.LCardDraftImage))
        {
            string imageId = LEngineImageResolve(images, draft);
            if (!attachedImages.Add(imageId))
            {
                continue;
            }

            if (collocation)
            {
                images.LImageCollocationAttach(ownerId, imageId, position);
            }
            else
            {
                images.LImageMeaningAttach(ownerId, imageId, position);
            }

            position++;
        }

        LVideoArchive videos = new(_lEngineDatabase);
        position = 0;
        HashSet<string> attachedVideos = new(StringComparer.Ordinal);
        foreach (LVideoDraft draft in LEngineVideoRead(card.LCardDraftVideo))
        {
            string videoId = LEngineVideoResolve(videos, draft);
            if (!attachedVideos.Add(videoId))
            {
                continue;
            }

            if (collocation)
            {
                videos.LVideoCollocationAttach(ownerId, videoId, position);
            }
            else
            {
                videos.LVideoMeaningAttach(ownerId, videoId, position);
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
                !string.IsNullOrWhiteSpace(card.LCardDraftGloss) ||
                LEngineSentenceCheck(card.LCardDraftSentence) ||
                LEngineSituationCheck(card.LCardDraftSituation) ||
                LEngineRegisterCheck(card.LCardDraftRegister) ||
                LEngineTagCheck(card.LCardDraftTag) ||
                LEngineTranslationCheck(card.LCardDraftTranslation) ||
                LEngineImageCheck(card.LCardDraftImage) ||
                LEngineVideoCheck(card.LCardDraftVideo) ||
                card.LCardDraftChild.Count > 0)
            {
                yield return card;
            }
        }
    }

    private static bool LEngineSentenceCheck(IReadOnlyList<LSentenceDraft> drafts)
    {
        foreach (LSentenceDraft draft in drafts)
        {
            if (!draft.LSentenceDraftEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<LSentenceDraft> LEngineSentenceRead(IReadOnlyList<LSentenceDraft> drafts)
    {
        foreach (LSentenceDraft draft in drafts)
        {
            if (!draft.LSentenceDraftEmpty)
            {
                yield return draft;
            }
        }
    }

    private static LExample? LEngineExampleResolve(
        LExampleArchive examples, LSentenceDraft draft, string language)
    {
        if (draft.LSentenceDraftExample is not LExampleDraft written
            || (written.LExampleDraftText.LStateValueEmpty && written.LExampleDraftId.Length == 0))
        {
            return null;
        }

        return LEngineExampleResolve(examples, written, language);
    }

    private static LExample LEngineExampleResolve(
        LExampleArchive examples, LExampleDraft written, string language)
    {
        if (!string.IsNullOrWhiteSpace(written.LExampleDraftId))
        {
            LExample? stored = examples.LExampleRead(written.LExampleDraftId);
            if (stored is not null)
            {
                if (stored.LExampleText != written.LExampleDraftText)
                {
                    examples.LExampleTextUpdate(stored.LExampleId, written.LExampleDraftText);
                    stored = stored with { LExampleText = written.LExampleDraftText };
                }

                if (stored.LExampleSource != written.LExampleDraftReference)
                {
                    examples.LExampleSourceUpdate(stored.LExampleId, written.LExampleDraftReference);
                    stored = stored with { LExampleSource = written.LExampleDraftReference };
                }

                return stored;
            }
        }

        return examples.LExampleCreate(new LExample(
            written.LExampleDraftId,
            written.LExampleDraftLanguage.Length == 0 ? language : written.LExampleDraftLanguage,
            written.LExampleDraftText,
            written.LExampleDraftTranslation,
            written.LExampleDraftReference));
    }

    private static bool LEngineSituationCheck(IReadOnlyList<LSituationDraft> drafts)
    {
        foreach (LSituationDraft draft in drafts)
        {
            if (!draft.LSituationDraftTitle.LStateValueEmpty)
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
            if (!draft.LSituationDraftTitle.LStateValueEmpty)
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
                LSituation written = stored with
                {
                    LSituationTitle = draft.LSituationDraftTitle,
                    LSituationDescription = draft.LSituationDraftDescription,
                    LSituationKind = draft.LSituationDraftKind,
                };

                if (written != stored)
                {
                    situations.LSituationUpdate(written);
                }

                return stored.LSituationId;
            }
        }

        return situations.LSituationCreate(new LSituation(
            draft.LSituationDraftId,
            draft.LSituationDraftTitle,
            draft.LSituationDraftDescription,
            draft.LSituationDraftKind)).LSituationId;
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

    private static bool LEngineVideoCheck(IReadOnlyList<LVideoDraft> rows)
    {
        foreach (LVideoDraft row in rows)
        {
            if (!row.LVideoDraftEmpty)
            {
                return true;
            }
        }

        return false;
    }

    internal static IEnumerable<LVideoDraft> LEngineVideoRead(IReadOnlyList<LVideoDraft> rows)
    {
        foreach (LVideoDraft row in rows)
        {
            if (!row.LVideoDraftEmpty)
            {
                yield return row;
            }
        }
    }

    private static bool LEngineImageCheck(IReadOnlyList<LImageDraft> rows)
    {
        foreach (LImageDraft row in rows)
        {
            if (!row.LImageDraftEmpty)
            {
                return true;
            }
        }

        return false;
    }

    internal static IEnumerable<LImageDraft> LEngineImageRead(IReadOnlyList<LImageDraft> rows)
    {
        foreach (LImageDraft row in rows)
        {
            if (!row.LImageDraftEmpty)
            {
                yield return row;
            }
        }
    }

    private static string LEngineImageResolve(LImageArchive images, LImageDraft draft)
    {
        if (!string.IsNullOrWhiteSpace(draft.LImageDraftId)
            && images.LImageRead(draft.LImageDraftId) is LImage stored)
        {
            if (stored.LImageLocation != draft.LImageDraftLocation)
            {
                images.LImageUpdate(stored with { LImageLocation = draft.LImageDraftLocation });
            }

            return stored.LImageId;
        }

        return images.LImageCreate(
            new LImage(string.Empty, draft.LImageDraftLocation)).LImageId;
    }

    private static string LEngineVideoResolve(LVideoArchive videos, LVideoDraft draft)
    {
        if (!string.IsNullOrWhiteSpace(draft.LVideoDraftId)
            && videos.LVideoRead(draft.LVideoDraftId) is LVideo stored)
        {
            LVideo written = stored with
            {
                LVideoLocation = draft.LVideoDraftLocation,
                LVideoSpan = draft.LVideoDraftSpan,
            };

            if (written != stored)
            {
                videos.LVideoUpdate(written);
            }

            return stored.LVideoId;
        }

        return videos.LVideoCreate(new LVideo(
            string.Empty, draft.LVideoDraftLocation, draft.LVideoDraftSpan)).LVideoId;
    }
}
