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

        if (collocation)
        {
            LEngineSynonymSync(ownerId, card.LCardDraftInterlink, identity);
        }
        else
        {
            LEngineRelationSync(ownerId, card.LCardDraftRelation, identity);
        }

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
                LEngineExampleResolve(exampleRows, draft, language, ownerId, collocation, identity),
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

    private void LEngineRelationSync(
        long ownerId, IReadOnlyList<LRelationDraft> drafts, Dictionary<long, long> identity)
    {
        LRelationArchive relations = new(_lEngineDatabase);
        Dictionary<long, LRelation> stored = [];
        foreach (LRelation row in relations.LRelationRead(ownerId))
        {
            stored[row.LRelationId] = row;
        }

        HashSet<long> kept = [];
        List<long> order = [];
        foreach (LRelationDraft draft in drafts)
        {
            if (draft.LRelationDraftEmpty || string.IsNullOrWhiteSpace(draft.LRelationDraftType))
            {
                continue;
            }

            LStateAnchor entry = LEngineAnchorSettle(draft.LRelationDraftEntry, identity, true);
            LStateAnchor meaning = LEngineAnchorSettle(draft.LRelationDraftMeaning, identity, false);
            if (entry.LStateAnchorEmpty == meaning.LStateAnchorEmpty)
            {
                continue;
            }

            LRelation written = new(
                draft.LRelationDraftId,
                ownerId,
                0,
                draft.LRelationDraftType,
                draft.LRelationDraftLabel,
                draft.LRelationDraftLabels,
                entry,
                meaning);

            long rowId;
            if (draft.LRelationDraftId > 0
                && stored.TryGetValue(draft.LRelationDraftId, out LRelation? row)
                && kept.Add(row.LRelationId))
            {
                if (row with { LRelationPosition = 0 } != written)
                {
                    relations.LRelationUpdate(written);
                }

                rowId = row.LRelationId;
            }
            else
            {
                rowId = relations.LRelationCreate(written with { LRelationId = 0 }).LRelationId;
                kept.Add(rowId);
                LEngineIdentityRecord(identity, draft.LRelationDraftId, rowId);
            }

            order.Add(rowId);
        }

        foreach (long rowId in stored.Keys)
        {
            if (!kept.Contains(rowId))
            {
                relations.LRelationDelete(rowId);
            }
        }

        relations.LRelationOrderSet(ownerId, order);
    }

    private void LEngineSynonymSync(
        long ownerId, IReadOnlyList<LSynonymDraft> drafts, Dictionary<long, long> identity)
    {
        LSynonymArchive synonyms = new(_lEngineDatabase);
        Dictionary<long, LSynonym> stored = [];
        foreach (LSynonym row in synonyms.LSynonymRead(ownerId))
        {
            stored[row.LSynonymId] = row;
        }

        HashSet<long> kept = [];
        List<long> order = [];
        foreach (LSynonymDraft draft in drafts)
        {
            if (draft.LSynonymDraftEmpty)
            {
                continue;
            }

            LStateAnchor entry = LEngineAnchorSettle(draft.LSynonymDraftEntry, identity, true);
            LStateAnchor meaning = LEngineAnchorSettle(draft.LSynonymDraftMeaning, identity, false);
            if (entry.LStateAnchorEmpty == meaning.LStateAnchorEmpty)
            {
                continue;
            }

            LSynonym written = new(draft.LSynonymDraftId, ownerId, 0, entry, meaning);

            long rowId;
            if (draft.LSynonymDraftId > 0
                && stored.TryGetValue(draft.LSynonymDraftId, out LSynonym? row)
                && kept.Add(row.LSynonymId))
            {
                if (row with { LSynonymPosition = 0 } != written)
                {
                    synonyms.LSynonymUpdate(written);
                }

                rowId = row.LSynonymId;
            }
            else
            {
                rowId = synonyms.LSynonymCreate(written with { LSynonymId = 0 }).LSynonymId;
                kept.Add(rowId);
                LEngineIdentityRecord(identity, draft.LSynonymDraftId, rowId);
            }

            order.Add(rowId);
        }

        foreach (long rowId in stored.Keys)
        {
            if (!kept.Contains(rowId))
            {
                synonyms.LSynonymDelete(rowId);
            }
        }

        synonyms.LSynonymOrderSet(ownerId, order);
    }

    private LStateAnchor LEngineAnchorSettle(LStateAnchor anchor, IReadOnlyDictionary<long, long> identity, bool entry)
    {
        if (anchor.LStateAnchorEmpty)
        {
            return anchor;
        }

        long id = anchor.LStateAnchorShow();
        if (id < 0)
        {
            return identity.TryGetValue(id, out long real)
                ? LStateAnchor.LStateAnchorCreate(real)
                : LStateAnchor.LStateAnchorUnspecified;
        }

        if (id == 0)
        {
            return LStateAnchor.LStateAnchorUnspecified;
        }

        bool present = entry
            ? new LEntryArchive(_lEngineDatabase).LEntryRead(id) is not null
            : new LMeaningArchive(_lEngineDatabase).LMeaningSingleRead(id) is not null;

        return present ? anchor : throw new LRefusal(LRefusal.LRefusalLink);
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
