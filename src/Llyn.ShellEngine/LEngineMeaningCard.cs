using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineMeaningUpdate(
        SqliteConnection connection,
        long entryId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        List<LRevisionChange> changes,
        Dictionary<long, long> identity)
    {
        LMeaningArchive meanings = new(_lEngineDatabase);

        Dictionary<long, LMeaning> stored = [];
        List<long> storedOrder = [];
        foreach (LMeaning row in meanings.LMeaningRead(entryId))
        {
            stored[row.LMeaningId] = row;
            storedOrder.Add(row.LMeaningId);
        }

        HashSet<long> named = [];
        LEngineMeaningScan(cards, stored, named);

        HashSet<long> gone = [];
        foreach (long dropped in storedOrder)
        {
            LMeaning row = stored[dropped];
            if (row.LMeaningParentId is long parent && gone.Contains(parent))
            {
                gone.Add(dropped);
                continue;
            }

            if (named.Contains(dropped))
            {
                continue;
            }

            gone.Add(dropped);
            changes.Add(new LRevisionChange(
                0, dropped, "sense", "delete", row.LMeaningDefinition.LStateValueShow()));
            meanings.LMeaningDelete(dropped);
        }

        LEngineMeaningApply(
            connection,
            entryId,
            null,
            cards,
            language,
            changes,
            meanings,
            stored,
            gone,
            new HashSet<long>(),
            identity);
    }

    private void LEngineMeaningApply(
        SqliteConnection connection,
        long entryId,
        long? parentId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        List<LRevisionChange> changes,
        LMeaningArchive meanings,
        IReadOnlyDictionary<long, LMeaning> stored,
        ISet<long> gone,
        ISet<long> applied,
        Dictionary<long, long> identity)
    {
        List<long> order = [];
        foreach (LCardDraft card in LEngineCardRead(cards))
        {
            bool reuse = stored.ContainsKey(card.LCardDraftId)
                && !gone.Contains(card.LCardDraftId)
                && applied.Add(card.LCardDraftId);

            long rowId;
            if (reuse)
            {
                LMeaning row = stored[card.LCardDraftId];
                if (row.LMeaningParentId != parentId)
                {
                    meanings.LMeaningParentUpdate(row.LMeaningId, parentId);
                }

                meanings.LMeaningUpdate(row with
                {
                    LMeaningTitle = card.LCardDraftTitle,
                    LMeaningDefinition = card.LCardDraftMeaning,
                });
                changes.Add(new LRevisionChange(
                    0, row.LMeaningId, "sense", "update", card.LCardDraftMeaning.LStateValueShow()));
                rowId = row.LMeaningId;
            }
            else
            {
                LMeaning created = meanings.LMeaningCreate(new LMeaning(
                0,
                    entryId,
                    parentId,
                    0,
                    card.LCardDraftTitle,
                    card.LCardDraftMeaning));
                changes.Add(new LRevisionChange(
                    0,
                    created.LMeaningId,
                    "sense",
                    "create",
                    card.LCardDraftMeaning.LStateValueShow()));
                rowId = created.LMeaningId;
                LEngineIdentityRecord(identity, card.LCardDraftId, rowId);
            }

            order.Add(rowId);
            LEngineCardSync(rowId, card, language, false, identity);
            LEngineMeaningApply(
                connection,
                entryId,
                rowId,
                card.LCardDraftChild,
                language,
                changes,
                meanings,
                stored,
                gone,
                applied,
                identity);
        }

        if (parentId is null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "sense", "entry_parent = $owner AND sense_parent IS NULL", entryId, "sense_id", order);
            return;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "sense", "sense_parent = $owner", parentId, "sense_id", order);
    }

    private static void LEngineMeaningScan(
        IReadOnlyList<LCardDraft> cards,
        IReadOnlyDictionary<long, LMeaning> stored,
        ISet<long> named)
    {
        foreach (LCardDraft card in LEngineCardRead(cards))
        {
            if (stored.ContainsKey(card.LCardDraftId))
            {
                named.Add(card.LCardDraftId);
            }

            LEngineMeaningScan(card.LCardDraftChild, stored, named);
        }
    }
}
