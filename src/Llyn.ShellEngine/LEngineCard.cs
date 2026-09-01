using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;

namespace Llyn.ShellEngine;

/// <summary>
/// The card half of <c>LEngineEntryUpdate</c>: reconciling an entry's stored Meanings and
/// Collocations, and the Examples, Situations and Tags they reference, to the cards a draft lists. It
/// sits in its own file because it is the bulk of the update and answers one question — which stored
/// row each card is, and what to do with the rows no card names any more.
/// </summary>
public sealed partial class LEngine
{
    // Reconciles one card set — the entry's Meanings or its Collocations — to the cards the draft
    // lists. A card naming a stored row of this entry updates that row, a card naming nothing creates
    // one, and a stored row the draft stopped naming is deleted; a card that has gone blank never
    // reaches here, so clearing a card removes its row exactly as adding text to a blank one adds one.
    //
    // Deletions run first so the creates that follow append onto a set already free of the rows that
    // are going, and the whole surviving set is renumbered afterwards in one pass: the unique
    // (owner, position) index rejects a swap done row by row, which is what LDatabaseOrder exists for.
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
                // A sub-meaning belongs to its parent's group, not to the entry's card list, and no
                // save writes one; leaving it out keeps this over the cards the form actually shows.
                if (row.LSenseParentId is not null)
                {
                    continue;
                }

                storedSenses[row.LSenseId] = row;
                storedOrder.Add(row.LSenseId);
            }
        }

        // The cards the draft still names, in draft order. A card naming a row of another entry — or
        // one already gone — names nothing here, so it is created rather than reaching across.
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
                    ? storedCollocations[dropped].LCollocationExpression
                    : storedSenses[dropped].LSenseDefinition));

            if (collocation)
            {
                collocations.LCollocationDelete(dropped);
            }
            else
            {
                senses.LSenseDelete(dropped);
            }
        }

        // A card names a stored row once: two cards carrying one id would otherwise both write that
        // row and the renumber would be handed the same member twice, so the second is a new card.
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

    // Writes a card the draft still names onto the row it names, keeping that row's id, and returns
    // the id. The columns the form has no control for — a Meaning's gloss, definition language and
    // labels — are carried over from the stored row rather than blanked by an edit that never saw them.
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
                0, row.LCollocationId, "collocation", "update", card.LCardDraftExpression));
            return row.LCollocationId;
        }

        LSense sense = storedSenses[card.LCardDraftId];
        senses.LSenseUpdate(sense with
        {
            LSenseTitle = card.LCardDraftTitle,
            LSenseDefinition = card.LCardDraftMeaning,
        });
        changes.Add(new LRevisionChange(0, sense.LSenseId, "sense", "update", card.LCardDraftMeaning));
        return sense.LSenseId;
    }

    // Writes a card the draft added as a new row and returns its fresh id. The row is appended; the
    // renumber that follows the whole set puts it where the draft holds it.
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
                0, row.LCollocationId, "collocation", "create", card.LCardDraftExpression));
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
        changes.Add(new LRevisionChange(0, sense.LSenseId, "sense", "create", card.LCardDraftMeaning));
        return sense.LSenseId;
    }

    // Re-attaches the Examples, Situations and Tags one card references so they match the draft. A row
    // whose text the card still lists is kept and moved to its new place, a text the card gained gets a
    // row of its own, and a row the card dropped is detached only — the rows are independent data the
    // card references, so the last reference going does not take the row with it. A newly created card
    // has nothing attached yet, so the same path attaches its whole set.
    private void LEngineCardSync(string ownerId, LCardDraft card, string language, bool collocation)
    {
        LExampleArchive exampleRows = new(_lEngineDatabase);
        LExampleLink examples = new(_lEngineDatabase);
        LEngineFieldSync(
            card.LCardDraftExample,
            collocation ? examples.LExampleCollocationRead(ownerId) : examples.LExampleSenseRead(ownerId),
            row => row.LExampleId,
            row => row.LExampleText,
            text => exampleRows.LExampleCreate(
                new LExample(string.Empty, language, text, null, null, [])).LExampleId,
            rowId =>
            {
                if (collocation)
                {
                    examples.LExampleCollocationDetach(ownerId, rowId);
                    return;
                }

                examples.LExampleSenseDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    examples.LExampleCollocationAttach(ownerId, rowId, position);
                    return;
                }

                examples.LExampleSenseAttach(ownerId, rowId, position);
            });

        LSituationArchive situations = new(_lEngineDatabase);
        LEngineFieldSync(
            card.LCardDraftSituation,
            collocation ? situations.LSituationCollocationRead(ownerId) : situations.LSituationSenseRead(ownerId),
            row => row.LSituationId,
            row => row.LSituationTitle,
            text => situations.LSituationCreate(
                new LSituation(string.Empty, text, null, null)).LSituationId,
            rowId =>
            {
                if (collocation)
                {
                    situations.LSituationCollocationDetach(ownerId, rowId);
                    return;
                }

                situations.LSituationSenseDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    situations.LSituationCollocationAttach(ownerId, rowId, position);
                    return;
                }

                situations.LSituationSenseAttach(ownerId, rowId, position);
            });

        LTagArchive tags = new(_lEngineDatabase);
        LEngineFieldSync(
            card.LCardDraftTag,
            collocation ? tags.LTagCollocationRead(ownerId) : tags.LTagSenseRead(ownerId),
            row => row.LTagId,
            row => row.LTagText,
            text => tags.LTagCreate(new LTag(string.Empty, text)).LTagId,
            rowId =>
            {
                if (collocation)
                {
                    tags.LTagCollocationDetach(ownerId, rowId);
                    return;
                }

                tags.LTagSenseDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    tags.LTagCollocationAttach(ownerId, rowId, position);
                    return;
                }

                tags.LTagSenseAttach(ownerId, rowId, position);
            });
    }

    // One card field reconciled against the rows it already references. Example, Situation and Tag
    // differ only in which store creates a row and which pair of methods attaches and detaches it, so
    // they are handed in and the reconciliation itself is written once.
    //
    // A value is matched to a referenced row by its text, which is what the shell can say about it: the
    // form holds typed text and no picker exists to name a row, so keeping the row a value already had
    // is the most an update can honour. Each row matches at most one value, so a card listing the same
    // text twice keeps one row and creates the second. Blank values are dropped on the same terms the
    // save drops them.
    private static void LEngineFieldSync<TRow>(
        IReadOnlyList<string> texts,
        IReadOnlyList<TRow> attached,
        Func<TRow, string> identify,
        Func<TRow, string?> read,
        Func<string, string> create,
        Action<string> detach,
        Action<string, int> attach)
    {
        List<string> targets = [];
        HashSet<string> kept = new(StringComparer.Ordinal);
        foreach (string text in LEngineFieldRead(texts))
        {
            string? found = null;
            foreach (TRow row in attached)
            {
                if (!kept.Contains(identify(row)) &&
                    string.Equals(read(row), text, StringComparison.Ordinal))
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

        // Attaching a row the card already references moves it: the association goes in at the end and
        // the set is renumbered around the requested position, so draft order becomes stored order.
        for (int position = 0; position < targets.Count; position++)
        {
            attach(targets[position], position);
        }
    }
}
