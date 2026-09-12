using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineCardUpdate(
        SqliteConnection connection,
        long entryId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        bool collocation,
        List<LRevisionChange> changes,
        Dictionary<long, long> identity)
    {
        LEngineCardValidate(cards, collocation);

        if (!collocation)
        {
            LEngineMeaningUpdate(connection, entryId, cards, language, changes, identity);
            return;
        }

        LCollocationArchive collocations = new(_lEngineDatabase);

        Dictionary<long, LCollocation> stored = [];
        List<long> storedOrder = [];
        foreach (LCollocation row in collocations.LCollocationRead(entryId))
        {
            stored[row.LCollocationId] = row;
            storedOrder.Add(row.LCollocationId);
        }

        List<LCardDraft> kept = [];
        HashSet<long> named = [];
        foreach (LCardDraft card in LEngineCardRead(cards))
        {
            kept.Add(card);
            if (stored.ContainsKey(card.LCardDraftId))
            {
                named.Add(card.LCardDraftId);
            }
        }

        foreach (long dropped in storedOrder)
        {
            if (named.Contains(dropped))
            {
                continue;
            }

            changes.Add(new LRevisionChange(
                0,
                dropped,
                "collocation",
                "delete",
                stored[dropped].LCollocationExpression.LStateValueShow()));

            collocations.LCollocationDelete(dropped);
        }

        HashSet<long> applied = [];
        List<long> order = [];
        foreach (LCardDraft card in kept)
        {
            bool reuse = named.Contains(card.LCardDraftId) && applied.Add(card.LCardDraftId);
            long rowId;
            if (reuse)
            {
                LCollocation row = stored[card.LCardDraftId];
                collocations.LCollocationUpdate(row with
                {
                    LCollocationTitle = card.LCardDraftTitle,
                    LCollocationExpression = card.LCardDraftExpression,
                    LCollocationMeaning = card.LCardDraftMeaning,
                });
                changes.Add(new LRevisionChange(
                    0,
                    row.LCollocationId,
                    "collocation",
                    "update",
                    card.LCardDraftExpression.LStateValueShow()));
                rowId = row.LCollocationId;
            }
            else
            {
                LCollocation row = collocations.LCollocationCreate(new LCollocation(
                0,
                    entryId,
                    0,
                    card.LCardDraftTitle,
                    card.LCardDraftExpression,
                    card.LCardDraftMeaning));
                changes.Add(new LRevisionChange(
                    0,
                    row.LCollocationId,
                    "collocation",
                    "create",
                    card.LCardDraftExpression.LStateValueShow()));
                rowId = row.LCollocationId;
                LEngineIdentityRecord(identity, card.LCardDraftId, rowId);
            }

            order.Add(rowId);
            LEngineCardSync(rowId, card, language, true, identity);
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation", "entry_id = $owner", entryId, "id", order);
    }

    private void LEngineCardSync(
        long ownerId, LCardDraft card, string language, bool collocation, Dictionary<long, long> identity)
    {
        LEngineSentenceSync(ownerId, card.LCardDraftSentence, language, collocation, identity);
        LEngineSituationSync(ownerId, card.LCardDraftSituation, collocation, identity);
        LEngineRegisterSync(ownerId, card.LCardDraftRegister, language, collocation, identity);

        LEngineTagSave(ownerId, card.LCardDraftTag, collocation, identity);
        LEngineTranslationSave(ownerId, card.LCardDraftTranslation, collocation);

        LImageArchive images = new(_lEngineDatabase);
        LEngineFieldSync(
            LEngineImageRead(card.LCardDraftImage),
            collocation ? images.LImageCollocationRead(ownerId) : images.LImageMeaningRead(ownerId),
            row => row.LImageId,
            written => LEngineImageResolve(images, written, identity),
            rowId =>
            {
                if (collocation)
                {
                    images.LImageCollocationDetach(ownerId, rowId);
                    return;
                }

                images.LImageMeaningDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    images.LImageCollocationAttach(ownerId, rowId, position);
                    return;
                }

                images.LImageMeaningAttach(ownerId, rowId, position);
            });

        LVideoArchive videos = new(_lEngineDatabase);
        LEngineFieldSync(
            LEngineVideoRead(card.LCardDraftVideo),
            collocation ? videos.LVideoCollocationRead(ownerId) : videos.LVideoMeaningRead(ownerId),
            row => row.LVideoId,
            written => LEngineVideoResolve(videos, written, identity),
            rowId =>
            {
                if (collocation)
                {
                    videos.LVideoCollocationDetach(ownerId, rowId);
                    return;
                }

                videos.LVideoMeaningDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    videos.LVideoCollocationAttach(ownerId, rowId, position);
                    return;
                }

                videos.LVideoMeaningAttach(ownerId, rowId, position);
            });
    }

    private void LEngineSentenceSync(
        long ownerId,
        IReadOnlyList<LSentenceDraft> drafts,
        string language,
        bool collocation,
        Dictionary<long, long> identity)
    {
        LExampleArchive exampleRows = new(_lEngineDatabase);

        List<LSentence> rows = [];
        foreach (LSentenceDraft draft in LEngineSentenceRead(drafts))
        {
            rows.Add(new LSentence(
                draft.LSentenceDraftId,
                ownerId,
                rows.Count,
                LEngineExampleResolve(exampleRows, draft, language, identity),
                draft.LSentenceDraftParticle,
                draft.LSentenceDraftDependence));
        }

        LSentenceArchive sentences = new(_lEngineDatabase);
        IReadOnlyList<long> written = collocation
            ? sentences.LSentenceCollocationSave(ownerId, rows)
            : sentences.LSentenceMeaningSave(ownerId, rows);

        for (int index = 0; index < rows.Count; index++)
        {
            LEngineIdentityRecord(identity, rows[index].LSentenceId, written[index]);
        }
    }

    private void LEngineSituationSync(
        long ownerId, IReadOnlyList<LSituationDraft> drafts, bool collocation, Dictionary<long, long> identity)
    {
        LSituationArchive situations = new(_lEngineDatabase);

        LEngineFieldSync(
            LEngineSituationRead(drafts),
            collocation ? situations.LSituationCollocationRead(ownerId) : situations.LSituationMeaningRead(ownerId),
            row => row.LSituationId,
            written => LEngineSituationResolve(situations, written, identity),
            rowId =>
            {
                if (collocation)
                {
                    situations.LSituationCollocationDetach(ownerId, rowId);
                    return;
                }

                situations.LSituationMeaningDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    situations.LSituationCollocationAttach(ownerId, rowId, position);
                    return;
                }

                situations.LSituationMeaningAttach(ownerId, rowId, position);
            });
    }

    private void LEngineTagSave(
        long ownerId, IReadOnlyList<LTagDraft> drafts, bool collocation, Dictionary<long, long> identity)
    {
        List<LTag> written = new(drafts.Count);
        foreach (LTagDraft draft in drafts)
        {
            written.Add(new LTag(draft.LTagDraftId, draft.LTagDraftText));
        }

        LTagArchive tags = new(_lEngineDatabase);
        IReadOnlyList<long> resolved = collocation
            ? tags.LTagCollocationSave(ownerId, written)
            : tags.LTagMeaningSave(ownerId, written);

        for (int index = 0; index < drafts.Count; index++)
        {
            if (resolved[index] != 0)
            {
                LEngineIdentityRecord(identity, drafts[index].LTagDraftId, resolved[index]);
            }
        }
    }

    private void LEngineTranslationSave(
        long ownerId, IReadOnlyList<long> ids, bool collocation)
    {
        LEntryArchive entries = new(_lEngineDatabase);
        List<LTranslation> written = new(ids.Count);
        foreach (long id in ids)
        {
            if (id > 0 && entries.LEntryRead(id) is not null)
            {
                written.Add(new LTranslation(id, 0));
            }
        }

        LTranslationArchive translations = new(_lEngineDatabase);
        if (collocation)
        {
            translations.LTranslationCollocationSave(ownerId, written);
            return;
        }

        translations.LTranslationMeaningSave(ownerId, written);
    }

    private static void LEngineFieldSync<LEngineRow, LEngineWritten>(
        IEnumerable<LEngineWritten> written,
        IReadOnlyList<LEngineRow> attached,
        Func<LEngineRow, long> identify,
        Func<LEngineWritten, long> resolve,
        Action<long> detach,
        Action<long, int> attach)
    {
        List<long> targets = [];
        HashSet<long> kept = [];
        foreach (LEngineWritten text in written)
        {
            long settled = resolve(text);
            if (!kept.Add(settled))
            {
                continue;
            }

            targets.Add(settled);
        }

        foreach (LEngineRow row in attached)
        {
            if (!kept.Contains(identify(row)))
            {
                detach(identify(row));
            }
        }

        for (int position = 0; position < targets.Count; position++)
        {
            attach(targets[position], position);
        }
    }
}
