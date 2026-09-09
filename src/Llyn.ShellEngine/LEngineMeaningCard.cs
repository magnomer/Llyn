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
        string entryId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        List<LRevisionChange> changes)
    {
        LMeaningArchive meanings = new(_lEngineDatabase);

        Dictionary<string, LMeaning> stored = new(StringComparer.Ordinal);
        List<string> storedOrder = [];
        foreach (LMeaning row in meanings.LMeaningRead(entryId))
        {
            stored[row.LMeaningId] = row;
            storedOrder.Add(row.LMeaningId);
        }

        HashSet<string> named = new(StringComparer.Ordinal);
        LEngineMeaningScan(cards, stored, named);

        HashSet<string> gone = new(StringComparer.Ordinal);
        foreach (string dropped in storedOrder)
        {
            LMeaning row = stored[dropped];
            if (row.LMeaningParentId is string parent && gone.Contains(parent))
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
            new HashSet<string>(StringComparer.Ordinal));
    }

    private void LEngineMeaningApply(
        SqliteConnection connection,
        string entryId,
        string? parentId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        List<LRevisionChange> changes,
        LMeaningArchive meanings,
        IReadOnlyDictionary<string, LMeaning> stored,
        ISet<string> gone,
        ISet<string> applied)
    {
        List<string> order = [];
        foreach (LCardDraft card in LEngineCardRead(cards))
        {
            bool reuse = stored.ContainsKey(card.LCardDraftId)
                && !gone.Contains(card.LCardDraftId)
                && applied.Add(card.LCardDraftId);

            string rowId;
            if (reuse)
            {
                LMeaning row = stored[card.LCardDraftId];
                if (!string.Equals(row.LMeaningParentId, parentId, StringComparison.Ordinal))
                {
                    meanings.LMeaningParentUpdate(row.LMeaningId, parentId);
                }

                meanings.LMeaningUpdate(row with
                {
                    LMeaningTitle = card.LCardDraftTitle,
                    LMeaningGloss = card.LCardDraftGloss,
                    LMeaningDefinitionLanguage = card.LCardDraftLanguage,
                    LMeaningDefinition = card.LCardDraftMeaning,
                    LMeaningLabels = card.LCardDraftLabels,
                });
                changes.Add(new LRevisionChange(
                    0, row.LMeaningId, "sense", "update", card.LCardDraftMeaning.LStateValueShow()));
                rowId = row.LMeaningId;
            }
            else
            {
                LMeaning created = meanings.LMeaningCreate(new LMeaning(
                    string.Empty,
                    entryId,
                    parentId,
                    0,
                    card.LCardDraftTitle,
                    card.LCardDraftGloss,
                    card.LCardDraftLanguage,
                    card.LCardDraftMeaning,
                    card.LCardDraftLabels));
                changes.Add(new LRevisionChange(
                    0,
                    created.LMeaningId,
                    "sense",
                    "create",
                    card.LCardDraftMeaning.LStateValueShow()));
                rowId = created.LMeaningId;
            }

            order.Add(rowId);
            LEngineCardSync(rowId, card, language, collocation: false);
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
                applied);
        }

        if (parentId is null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "sense", "entry_id = $owner AND parent_id IS NULL", entryId, "id", order);
            return;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "sense", "parent_id = $owner", parentId, "id", order);
    }

    private static void LEngineMeaningScan(
        IReadOnlyList<LCardDraft> cards,
        IReadOnlyDictionary<string, LMeaning> stored,
        ISet<string> named)
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
