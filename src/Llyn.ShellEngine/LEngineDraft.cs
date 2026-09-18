using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntrySave(LEntryDraft draft)
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

            draft = LEngineRespellingUpdate(LEngineDraftNormalize(draft) with
            {
                LEntryDraftHeadword = draft.LEntryDraftHeadword.Trim(),
            });

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEntry entry = new LEntryArchive(_lEngineDatabase).LEntryCreate(
                new LEntry(
                    0,
                    draft.LEntryDraftHeadword,
                    draft.LEntryDraftLanguage,
                    0,
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

            string note = LMarkdown.LMarkdownNormalize(draft.LEntryDraftNote);

            if (note.Length > 0)
            {
                new LNoteArchive(_lEngineDatabase).LNoteSave(new LNote(entry.LEntryId, note));
            }

            LEnginePronunciationSync(
                entry.LEntryId, LEnginePronunciationReset(draft.LEntryDraftPronunciations), null, identity);
            LEngineTranscriptionSync(
                entry.LEntryId, LEngineTranscriptionReset(draft.LEntryDraftTranscriptions), null, identity);
            LEngineReflexSync(
                entry.LEntryId,
                draft.LEntryDraftLanguage,
                LEngineReflexReset(draft.LEntryDraftReflexes),
                null,
                identity);
            LEngineParadigmUpdate(entry);

            LRevisionChange change = new(0, entry.LEntryId, "entry", "create", entry.LEntryHeadword);
            LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);

            LWorkspaceArchive workspace = new(_lEngineDatabase);
            LWorkspaceState state = workspace.LWorkspaceStateRead();
            workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

            session.LDatabaseSessionCommit();
            LEngineFrequencyStart(entry.LEntryId);
            return entry;
        }
    }

    private static IReadOnlyList<LPronunciationDraft> LEnginePronunciationReset(
        IReadOnlyList<LPronunciationDraft> drafts)
    {
        List<LPronunciationDraft> renewed = new(drafts.Count);
        foreach (LPronunciationDraft draft in drafts)
        {
            renewed.Add(draft.LPronunciationDraftId > 0 ? draft with { LPronunciationDraftId = 0 } : draft);
        }

        return renewed;
    }

    private static IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionReset(
        IReadOnlyList<LTranscriptionDraft> drafts)
    {
        List<LTranscriptionDraft> renewed = new(drafts.Count);
        foreach (LTranscriptionDraft draft in drafts)
        {
            renewed.Add(draft.LTranscriptionDraftId > 0 ? draft with { LTranscriptionDraftId = 0 } : draft);
        }

        return renewed;
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
            card.LCardDraftMeaning));

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
