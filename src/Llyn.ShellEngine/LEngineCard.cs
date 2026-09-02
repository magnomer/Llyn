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
        LSenseArchive senses = new(_lEngineDatabase);
        LCollocationArchive collocations = new(_lEngineDatabase);

        Dictionary<string, LSense> storedSenses = new(StringComparer.Ordinal);
        Dictionary<string, LCollocation> storedCollocations = new(StringComparer.Ordinal);
        List<string> storedOrder = [];
        if (collocation)
        {
            foreach (LCollocation row in collocations.LCollocationRead(entryId))
            {
                storedCollocations[row.LCollocationId] = row;
                storedOrder.Add(row.LCollocationId);
            }
        }
        else
        {
            foreach (LSense row in senses.LSenseRead(entryId))
            {
                if (row.LSenseParentId is not null)
                {
                    continue;
                }

                storedSenses[row.LSenseId] = row;
                storedOrder.Add(row.LSenseId);
            }
        }

        List<LCardDraft> kept = [];
        HashSet<string> named = new(StringComparer.Ordinal);
        foreach (LCardDraft card in LEngineCardRead(cards))
        {
            kept.Add(card);
            bool known = collocation
                ? storedCollocations.ContainsKey(card.LCardDraftId)
                : storedSenses.ContainsKey(card.LCardDraftId);
            if (known)
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
                collocation ? "collocation" : "sense",
                "delete",
                collocation
                    ? storedCollocations[dropped].LCollocationExpression.LStateValueShow()
                    : storedSenses[dropped].LSenseDefinition.LStateValueShow()));

            if (collocation)
            {
                collocations.LCollocationDelete(dropped);
            }
            else
            {
                senses.LSenseDelete(dropped);
            }
        }

        HashSet<string> applied = new(StringComparer.Ordinal);
        List<string> order = [];
        foreach (LCardDraft card in kept)
        {
            bool reuse = named.Contains(card.LCardDraftId) && applied.Add(card.LCardDraftId);
            string rowId = reuse
                ? LEngineCardApply(senses, collocations, card, collocation, storedSenses, storedCollocations, changes)
                : LEngineCardCreate(senses, collocations, entryId, card, collocation, changes);

            order.Add(rowId);
            LEngineCardSync(rowId, card, language, collocation);
        }

        if (collocation)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "collocation", "entry_id = $owner", entryId, "id", order);
            return;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "sense", "entry_id = $owner AND parent_id IS NULL", entryId, "id", order);
    }

    private static string LEngineCardApply(
        LSenseArchive senses,
        LCollocationArchive collocations,
        LCardDraft card,
        bool collocation,
        IReadOnlyDictionary<string, LSense> storedSenses,
        IReadOnlyDictionary<string, LCollocation> storedCollocations,
        List<LRevisionChange> changes)
    {
        if (collocation)
        {
            LCollocation row = storedCollocations[card.LCardDraftId];
            collocations.LCollocationUpdate(row with
            {
                LCollocationTitle = card.LCardDraftTitle,
                LCollocationExpression = card.LCardDraftExpression,
                LCollocationMeaning = card.LCardDraftMeaning,
            });
            changes.Add(new LRevisionChange(
                0, row.LCollocationId, "collocation", "update", card.LCardDraftExpression.LStateValueShow()));
            return row.LCollocationId;
        }

        LSense sense = storedSenses[card.LCardDraftId];
        senses.LSenseUpdate(sense with
        {
            LSenseTitle = card.LCardDraftTitle,
            LSenseDefinition = card.LCardDraftMeaning,
        });
        changes.Add(new LRevisionChange(
            0, sense.LSenseId, "sense", "update", card.LCardDraftMeaning.LStateValueShow()));
        return sense.LSenseId;
    }

    private static string LEngineCardCreate(
        LSenseArchive senses,
        LCollocationArchive collocations,
        string entryId,
        LCardDraft card,
        bool collocation,
        List<LRevisionChange> changes)
    {
        if (collocation)
        {
            LCollocation row = collocations.LCollocationCreate(new LCollocation(
                string.Empty,
                entryId,
                0,
                card.LCardDraftTitle,
                card.LCardDraftExpression,
                card.LCardDraftMeaning));
            changes.Add(new LRevisionChange(
                0, row.LCollocationId, "collocation", "create", card.LCardDraftExpression.LStateValueShow()));
            return row.LCollocationId;
        }

        LSense sense = senses.LSenseCreate(new LSense(
            string.Empty,
            entryId,
            null,
            0,
            card.LCardDraftTitle,
            null,
            null,
            card.LCardDraftMeaning,
            string.Empty));
        changes.Add(new LRevisionChange(
            0, sense.LSenseId, "sense", "create", card.LCardDraftMeaning.LStateValueShow()));
        return sense.LSenseId;
    }

    private void LEngineCardSync(string ownerId, LCardDraft card, string language, bool collocation)
    {
        LEngineExampleSync(ownerId, card.LCardDraftExample, language, collocation);
        LEngineSituationSync(ownerId, card.LCardDraftSituation, collocation);

        LEngineTagSave(ownerId, card.LCardDraftTag, collocation);

        LImageArchive images = new(_lEngineDatabase);
        LEngineFieldSync(
            card.LCardDraftImage,
            collocation ? images.LImageCollocationRead(ownerId) : images.LImageSenseRead(ownerId),
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

                images.LImageSenseDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    images.LImageCollocationAttach(ownerId, rowId, position);
                    return;
                }

                images.LImageSenseAttach(ownerId, rowId, position);
            });
    }

    private void LEngineExampleSync(
        string ownerId, IReadOnlyList<LExampleDraft> drafts, string language, bool collocation)
    {
        LExampleArchive exampleRows = new(_lEngineDatabase);
        LExampleLink examples = new(_lEngineDatabase);

        IReadOnlyList<LExample> attached = collocation
            ? examples.LExampleCollocationRead(ownerId)
            : examples.LExampleSenseRead(ownerId);

        List<string> targets = [];
        HashSet<string> kept = new(StringComparer.Ordinal);
        foreach (LExampleDraft draft in LEngineExampleRead(drafts))
        {
            string id = LEngineExampleResolve(exampleRows, draft, language);
            if (!kept.Add(id))
            {
                continue;
            }

            targets.Add(id);
        }

        foreach (LExample row in attached)
        {
            if (kept.Contains(row.LExampleId))
            {
                continue;
            }

            if (collocation)
            {
                examples.LExampleCollocationDetach(ownerId, row.LExampleId);
                continue;
            }

            examples.LExampleSenseDetach(ownerId, row.LExampleId);
        }

        for (int position = 0; position < targets.Count; position++)
        {
            if (collocation)
            {
                examples.LExampleCollocationAttach(ownerId, targets[position], position);
                continue;
            }

            examples.LExampleSenseAttach(ownerId, targets[position], position);
        }
    }

    private void LEngineSituationSync(
        string ownerId, IReadOnlyList<LSituationDraft> drafts, bool collocation)
    {
        LSituationArchive situations = new(_lEngineDatabase);

        IReadOnlyList<LSituation> attached = collocation
            ? situations.LSituationCollocationRead(ownerId)
            : situations.LSituationSenseRead(ownerId);

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

            situations.LSituationSenseDetach(ownerId, row.LSituationId);
        }

        for (int position = 0; position < targets.Count; position++)
        {
            if (collocation)
            {
                situations.LSituationCollocationAttach(ownerId, targets[position], position);
                continue;
            }

            situations.LSituationSenseAttach(ownerId, targets[position], position);
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

        tags.LTagSenseSave(ownerId, written);
    }

    private static void LEngineFieldSync<TRow>(
        IReadOnlyList<LStateValue> texts,
        IReadOnlyList<TRow> attached,
        Func<TRow, string> identify,
        Func<TRow, LStateValue> read,
        Func<LStateValue, string> create,
        Action<string> detach,
        Action<string, int> attach)
    {
        List<string> targets = [];
        HashSet<string> kept = new(StringComparer.Ordinal);
        foreach (LStateValue text in LEngineFieldRead(texts))
        {
            string? found = null;
            foreach (TRow row in attached)
            {
                if (!kept.Contains(identify(row)) && read(row) == text)
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
