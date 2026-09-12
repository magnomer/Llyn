using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LEntry LEngineEntrySave(LEntryDraft draft)
    {
        return LEngineEntrySave(draft, []);
    }

    private LEntry LEngineEntrySave(LEntryDraft draft, Dictionary<long, long> identity)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(draft);

            if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
            {
                throw new LRefusal(LRefusal.LRefusalHeadword);
            }

            draft = LEngineDraftNormalize(draft);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(
                new LEntry(
                    0,
                    draft.LEntryDraftHeadword,
                    draft.LEntryDraftLanguage,
                    null,
                    null,
                    null,
                    null),
                forms: draft.LEntryDraftForms,
                speeches: LEngineSpeechResolve(
                    0, draft.LEntryDraftLanguage, draft.LEntryDraftSpeeches));

            if (draft.LEntryDraftInflections.Count > 0)
            {
                LEngineInflectionValidate(draft.LEntryDraftInflections);
                new LInflectionArchive(_lEngineDatabase).LInflectionSet(
                    entry.LEntryId, draft.LEntryDraftInflections);
            }

            LEngineCardValidate(draft.LEntryDraftMeanings, collocation: false);
            LEngineCardValidate(draft.LEntryDraftCollocations, collocation: true);

            foreach (LCardDraft card in LEngineCardRead(draft.LEntryDraftMeanings))
            {
                LEngineMeaningCreate(entry.LEntryId, null, card, draft.LEntryDraftLanguage, identity);
            }

            LCollocationArchive collocations = new(_lEngineDatabase);
            foreach (LCardDraft card in LEngineCardRead(draft.LEntryDraftCollocations))
            {
                LCollocation collocation = collocations.LCollocationCreate(new LCollocation(
                0,
                    entry.LEntryId,
                    0,
                    card.LCardDraftTitle,
                    card.LCardDraftExpression,
                    card.LCardDraftMeaning));

                LEngineIdentityRecord(identity, card.LCardDraftId, collocation.LCollocationId);
                LEngineCardSync(
                    collocation.LCollocationId, card, draft.LEntryDraftLanguage, true, identity);
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
                0,
                    entry.LEntryId,
                    spoken.LPronunciationDraftLevel,
                    spoken.LPronunciationDraftIpa,
                    spoken.LPronunciationDraftSyllables,
                    spoken.LPronunciationDraftRepresentations));

                LEngineIdentityRecord(identity, spoken.LPronunciationDraftId, pronunciation.LPronunciationId);
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
        long entryId, long? parentId, LCardDraft card, string language, Dictionary<long, long> identity)
    {
        LMeaning meaning = new LMeaningArchive(_lEngineDatabase).LMeaningCreate(new LMeaning(
                0,
            entryId,
            parentId,
            0,
            card.LCardDraftTitle,
            card.LCardDraftGloss,
            card.LCardDraftLanguage,
            card.LCardDraftMeaning,
            card.LCardDraftLabels));

        LEngineIdentityRecord(identity, card.LCardDraftId, meaning.LMeaningId);
        LEngineCardSync(meaning.LMeaningId, card, language, false, identity);

        foreach (LCardDraft child in LEngineCardRead(card.LCardDraftChild))
        {
            LEngineMeaningCreate(entryId, meaning.LMeaningId, child, language, identity);
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

    private static IEnumerable<LCardDraft> LEngineCardRead(IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            if (!card.LCardDraftEmpty)
            {
                yield return card;
            }
        }
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

    private LExample? LEngineExampleResolve(
        LExampleArchive examples,
        LSentenceDraft draft,
        string language,
        long ownerId,
        bool collocation,
        Dictionary<long, long> identity)
    {
        if (draft.LSentenceDraftExample is not LExampleDraft written
            || (written.LExampleDraftText.LStateValueEmpty && written.LExampleDraftId <= 0))
        {
            return null;
        }

        return LEngineExampleResolve(examples, written, language, ownerId, collocation, identity);
    }

    private LExample LEngineExampleResolve(
        LExampleArchive examples,
        LExampleDraft written,
        string language,
        long ownerId,
        bool collocation,
        Dictionary<long, long> identity)
    {
        if (written.LExampleDraftId > 0)
        {
            LExample stored = examples.LExampleRead(written.LExampleDraftId)
                ?? throw new LRefusal(LRefusal.LRefusalLink);

            bool sameText = stored.LExampleText == written.LExampleDraftText;
            bool sameSource = stored.LExampleSource == written.LExampleDraftReference;
            if (sameText && sameSource)
            {
                return stored;
            }

            if (LEngineExampleShareCheck(stored.LExampleId, ownerId, collocation))
            {
                return examples.LExampleCreate(new LExample(
                    0,
                    stored.LExampleLanguage,
                    written.LExampleDraftText,
                    stored.LExampleTranslation,
                    written.LExampleDraftReference));
            }

            if (!sameText)
            {
                examples.LExampleTextUpdate(stored.LExampleId, written.LExampleDraftText);
                stored = stored with { LExampleText = written.LExampleDraftText };
            }

            if (!sameSource)
            {
                examples.LExampleSourceUpdate(stored.LExampleId, written.LExampleDraftReference);
                stored = stored with { LExampleSource = written.LExampleDraftReference };
            }

            return stored;
        }

        LExample created = examples.LExampleCreate(new LExample(
            0,
            written.LExampleDraftLanguage.Length == 0 ? language : written.LExampleDraftLanguage,
            written.LExampleDraftText,
            written.LExampleDraftTranslation,
            written.LExampleDraftReference));
        LEngineIdentityRecord(identity, written.LExampleDraftId, created.LExampleId);
        return created;
    }

    private bool LEngineExampleShareCheck(long exampleId, long ownerId, bool collocation)
    {
        LOwner owner = collocation ? LOwner.LOwnerCollocation : LOwner.LOwnerMeaning;
        foreach (LUsage usage in new LExampleLink(_lEngineDatabase).LExampleUsageRead(exampleId))
        {
            if (usage.LUsageId != ownerId || usage.LUsageOwner != owner)
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

    private static long LEngineSituationResolve(
        LSituationArchive situations, LSituationDraft draft, Dictionary<long, long> identity)
    {
        if (draft.LSituationDraftId > 0)
        {
            LSituation stored = situations.LSituationRead(draft.LSituationDraftId)
                ?? throw new LRefusal(LRefusal.LRefusalLink);
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

        LSituation? found = LEngineSituationResolve(situations, draft.LSituationDraftTitle);
        if (found is not null)
        {
            return found.LSituationId;
        }

        long created = situations.LSituationCreate(new LSituation(
            0,
            draft.LSituationDraftTitle,
            draft.LSituationDraftDescription,
            draft.LSituationDraftKind)).LSituationId;
        LEngineIdentityRecord(identity, draft.LSituationDraftId, created);
        return created;
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

    private static long LEngineImageResolve(
        LImageArchive images, LImageDraft draft, Dictionary<long, long> identity)
    {
        if (draft.LImageDraftId > 0)
        {
            LImage stored = images.LImageRead(draft.LImageDraftId)
                ?? throw new LRefusal(LRefusal.LRefusalLink);
            if (stored.LImageLocation != draft.LImageDraftLocation)
            {
                images.LImageUpdate(stored with { LImageLocation = draft.LImageDraftLocation });
            }

            return stored.LImageId;
        }

        long created = images.LImageCreate(new LImage(0, draft.LImageDraftLocation)).LImageId;
        LEngineIdentityRecord(identity, draft.LImageDraftId, created);
        return created;
    }

    private static long LEngineVideoResolve(
        LVideoArchive videos, LVideoDraft draft, Dictionary<long, long> identity)
    {
        if (draft.LVideoDraftId > 0)
        {
            LVideo stored = videos.LVideoRead(draft.LVideoDraftId)
                ?? throw new LRefusal(LRefusal.LRefusalLink);
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

        long created = videos.LVideoCreate(new LVideo(
            0, draft.LVideoDraftLocation, draft.LVideoDraftSpan)).LVideoId;
        LEngineIdentityRecord(identity, draft.LVideoDraftId, created);
        return created;
    }
}
