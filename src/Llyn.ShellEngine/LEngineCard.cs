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
        string entryId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        bool collocation,
        List<LRevisionChange> changes)
    {
        LEngineCardValidate(cards, collocation);

        if (!collocation)
        {
            LEngineMeaningUpdate(connection, entryId, cards, language, changes);
            return;
        }

        LCollocationArchive collocations = new(_lEngineDatabase);

        Dictionary<string, LCollocation> stored = new(StringComparer.Ordinal);
        List<string> storedOrder = [];
        foreach (LCollocation row in collocations.LCollocationRead(entryId))
        {
            stored[row.LCollocationId] = row;
            storedOrder.Add(row.LCollocationId);
        }

        List<LCardDraft> kept = [];
        HashSet<string> named = new(StringComparer.Ordinal);
        foreach (LCardDraft card in LEngineCardRead(cards))
        {
            kept.Add(card);
            if (stored.ContainsKey(card.LCardDraftId))
            {
                named.Add(card.LCardDraftId);
            }
        }

        foreach (string dropped in storedOrder)
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

        HashSet<string> applied = new(StringComparer.Ordinal);
        List<string> order = [];
        foreach (LCardDraft card in kept)
        {
            bool reuse = named.Contains(card.LCardDraftId) && applied.Add(card.LCardDraftId);
            string rowId;
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
                    string.Empty,
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
            }

            order.Add(rowId);
            LEngineCardSync(rowId, card, language, collocation: true);
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation", "entry_id = $owner", entryId, "id", order);
    }

    private void LEngineCardSync(string ownerId, LCardDraft card, string language, bool collocation)
    {
        LEngineSentenceSync(ownerId, card.LCardDraftSentence, language, collocation);
        LEngineSituationSync(ownerId, card.LCardDraftSituation, collocation);
        LEngineRegisterSync(ownerId, card.LCardDraftRegister, language, collocation);

        LEngineTagSave(ownerId, card.LCardDraftTag, collocation);
        LEngineTranslationSave(ownerId, card.LCardDraftTranslation, collocation);

        LImageArchive images = new(_lEngineDatabase);
        LEngineFieldSync(
            LEngineFieldRead(card.LCardDraftImage),
            collocation ? images.LImageCollocationRead(ownerId) : images.LImageMeaningRead(ownerId),
            row => row.LImageId,
            row => row.LImageLocation,
            location => images.LImageCreate(new LImage(string.Empty, location)).LImageId,
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
            row => new LVideoDraft(row.LVideoLocation, row.LVideoSpan),
            written => videos.LVideoCreate(
                new LVideo(string.Empty, written.LVideoDraftLocation, written.LVideoDraftSpan)).LVideoId,
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
        string ownerId, IReadOnlyList<LSentenceDraft> drafts, string language, bool collocation)
    {
        LExampleArchive exampleRows = new(_lEngineDatabase);

        List<LSentence> rows = [];
        foreach (LSentenceDraft draft in LEngineSentenceRead(drafts))
        {
            rows.Add(new LSentence(
                draft.LSentenceDraftId,
                ownerId,
                rows.Count,
                LEngineExampleResolve(exampleRows, draft, language),
                draft.LSentenceDraftParticle,
                draft.LSentenceDraftDependence));
        }

        LSentenceArchive sentences = new(_lEngineDatabase);
        if (collocation)
        {
            sentences.LSentenceCollocationSave(ownerId, rows);
            return;
        }

        sentences.LSentenceMeaningSave(ownerId, rows);
    }

    private void LEngineSituationSync(
        string ownerId, IReadOnlyList<LSituationDraft> drafts, bool collocation)
    {
        LSituationArchive situations = new(_lEngineDatabase);

        IReadOnlyList<LSituation> attached = collocation
            ? situations.LSituationCollocationRead(ownerId)
            : situations.LSituationMeaningRead(ownerId);

        List<string> targets = [];
        HashSet<string> kept = new(StringComparer.Ordinal);
        foreach (LSituationDraft draft in LEngineSituationRead(drafts))
        {
            string id = LEngineSituationResolve(situations, draft);
            if (!kept.Add(id))
            {
                continue;
            }

            targets.Add(id);
        }

        foreach (LSituation row in attached)
        {
            if (kept.Contains(row.LSituationId))
            {
                continue;
            }

            if (collocation)
            {
                situations.LSituationCollocationDetach(ownerId, row.LSituationId);
                continue;
            }

            situations.LSituationMeaningDetach(ownerId, row.LSituationId);
        }

        for (int position = 0; position < targets.Count; position++)
        {
            if (collocation)
            {
                situations.LSituationCollocationAttach(ownerId, targets[position], position);
                continue;
            }

            situations.LSituationMeaningAttach(ownerId, targets[position], position);
        }
    }

    private void LEngineTagSave(string ownerId, IReadOnlyList<string> texts, bool collocation)
    {
        List<LTag> written = new(texts.Count);
        foreach (string text in texts)
        {
            written.Add(new LTag(text));
        }

        LTagArchive tags = new(_lEngineDatabase);
        if (collocation)
        {
            tags.LTagCollocationSave(ownerId, written);
            return;
        }

        tags.LTagMeaningSave(ownerId, written);
    }

    private void LEngineTranslationSave(
        string ownerId, IReadOnlyList<string> ids, bool collocation)
    {
        List<LTranslation> written = new(ids.Count);
        foreach (string id in ids)
        {
            written.Add(new LTranslation(id, 0));
        }

        LTranslationArchive translations = new(_lEngineDatabase);
        if (collocation)
        {
            translations.LTranslationCollocationSave(ownerId, written);
            return;
        }

        translations.LTranslationMeaningSave(ownerId, written);
    }

    private static void LEngineFieldSync<TRow, TWritten>(
        IEnumerable<TWritten> written,
        IReadOnlyList<TRow> attached,
        Func<TRow, string> identify,
        Func<TRow, TWritten> read,
        Func<TWritten, string> create,
        Action<string> detach,
        Action<string, int> attach)
    {
        List<string> targets = [];
        HashSet<string> kept = new(StringComparer.Ordinal);
        foreach (TWritten text in written)
        {
            string? found = null;
            foreach (TRow row in attached)
            {
                if (!kept.Contains(identify(row))
                    && EqualityComparer<TWritten>.Default.Equals(read(row), text))
                {
                    found = identify(row);
                    break;
                }
            }

            found ??= create(text);
            kept.Add(found);
            targets.Add(found);
        }

        foreach (TRow row in attached)
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
