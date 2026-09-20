using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCardClerk
{
    private readonly LVault _lCardClerkVault;
    private readonly LEntryVault _lCardClerkEntries;
    private readonly LCollocationVault _lCardClerkCollocations;
    private readonly LMeaningVault _lCardClerkMeanings;
    private readonly LImageVault _lCardClerkImages;
    private readonly LVideoVault _lCardClerkVideos;
    private readonly LSentenceVault _lCardClerkSentences;
    private readonly LSituationVault _lCardClerkContexts;
    private readonly LTagClerk _lCardClerkTags;
    private readonly LRegisterClerk _lCardClerkRegisters;
    private readonly LTranslationClerk _lCardClerkTranslations;
    private readonly LExampleClerk _lCardClerkExamples;
    private readonly LSituationClerk _lCardClerkSituations;

    public LCardClerk(
        LRig rig,
        LTagClerk tags,
        LRegisterClerk registers,
        LTranslationClerk translations,
        LExampleClerk examples,
        LSituationClerk situations)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(registers);
        ArgumentNullException.ThrowIfNull(translations);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(situations);
        _lCardClerkVault = rig.LRigVault;
        _lCardClerkEntries = rig.LRigEntries;
        _lCardClerkCollocations = rig.LRigCollocations;
        _lCardClerkMeanings = rig.LRigMeanings;
        _lCardClerkImages = rig.LRigImages;
        _lCardClerkVideos = rig.LRigVideos;
        _lCardClerkSentences = rig.LRigSentences;
        _lCardClerkContexts = rig.LRigSituations;
        _lCardClerkTags = tags;
        _lCardClerkRegisters = registers;
        _lCardClerkTranslations = translations;
        _lCardClerkExamples = examples;
        _lCardClerkSituations = situations;
    }

    public static bool LCardOwnerCheck(LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerMeaning => false,
            LOwner.LOwnerCollocation => true,
            _ => throw new ArgumentOutOfRangeException(
                nameof(owner), owner, "This entity has no reference from that kind of row."),
        };
    }

    public void LExampleRemove(long ownerId, long exampleId, LOwner owner)
    {
        using LVaultSession session = _lCardClerkVault.LVaultSessionStart();
        _lCardClerkExamples.LExampleClerkRemove(ownerId, exampleId, owner);
        LCardUpdatedSet(ownerId, LCardOwnerCheck(owner));
        session.LVaultSessionCommit();
    }

    public void LSituationRemove(long ownerId, long situationId, LOwner owner)
    {
        using LVaultSession session = _lCardClerkVault.LVaultSessionStart();
        _lCardClerkSituations.LSituationClerkRemove(ownerId, situationId, owner);
        LCardUpdatedSet(ownerId, LCardOwnerCheck(owner));
        session.LVaultSessionCommit();
    }

    public LCollocation LCollocationCreate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        LCollocation created = _lCardClerkCollocations.LCollocationCreate(collocation);
        _lCardClerkEntries.LEntryUpdatedSet(created.LCollocationEntryId);
        return created;
    }

    public IReadOnlyList<LCollocation> LCollocationRead(long entryId)
    {
        return _lCardClerkCollocations.LCollocationRead(entryId);
    }

    public void LCollocationUpdate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        _lCardClerkCollocations.LCollocationUpdate(collocation);
        _lCardClerkEntries.LEntryUpdatedSet(collocation.LCollocationEntryId);
    }

    public void LCollocationMove(long id, int position)
    {
        _lCardClerkCollocations.LCollocationMove(id, position);
        LCardUpdatedSet(id, true);
    }

    public void LCollocationDelete(long id)
    {
        LCollocationVault collocations = _lCardClerkCollocations;
        long? entryId = collocations.LCollocationHolderRead(id);
        collocations.LCollocationDelete(id);
        if (entryId is long held)
        {
            _lCardClerkEntries.LEntryUpdatedSet(held);
        }
    }

    public void LCardUpdatedSet(long ownerId, bool collocation)
    {
        long? entryId = collocation
            ? _lCardClerkCollocations.LCollocationHolderRead(ownerId)
            : _lCardClerkMeanings.LMeaningHolderRead(ownerId);

        if (entryId is long held)
        {
            _lCardClerkEntries.LEntryUpdatedSet(held);
        }
    }

    public void LCollocationSave(
        long entryId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        List<LRevisionChange> changes,
        Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(changes);
        ArgumentNullException.ThrowIfNull(identity);

        LCardClerkField.LCardValidate(cards, true);

        LCollocationVault collocations = _lCardClerkCollocations;

        Dictionary<long, LCollocation> stored = [];
        List<long> storedOrder = [];
        foreach (LCollocation row in collocations.LCollocationRead(entryId))
        {
            stored[row.LCollocationId] = row;
            storedOrder.Add(row.LCollocationId);
        }

        List<LCardDraft> kept = [];
        HashSet<long> named = [];
        foreach (LCardDraft card in LCardClerkField.LCardRead(cards))
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
                if (row.LCollocationTitle != card.LCardDraftTitle
                    || row.LCollocationExpression != card.LCardDraftExpression
                    || row.LCollocationMeaning != card.LCardDraftMeaning
                    || row.LCollocationPosition != order.Count
                    || card.LCardDraftTitle.LStateValueUnreadable
                    || card.LCardDraftExpression.LStateValueUnreadable
                    || card.LCardDraftMeaning.LStateValueUnreadable)
                {
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
                }

                rowId = row.LCollocationId;
            }
            else
            {
                rowId = LCollocationInsert(entryId, card, identity);
                changes.Add(new LRevisionChange(
                    0,
                    rowId,
                    "collocation",
                    "create",
                    card.LCardDraftExpression.LStateValueShow()));
            }

            order.Add(rowId);
            LCardClerkSync(rowId, card, language, true, identity);
        }

        collocations.LCollocationOrderSet(entryId, order);
    }

    public long LCollocationInsert(long entryId, LCardDraft card, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(card);

        LCollocation row = _lCardClerkCollocations.LCollocationCreate(new LCollocation(
            0,
            entryId,
            0,
            card.LCardDraftTitle,
            card.LCardDraftExpression,
            card.LCardDraftMeaning));

        LIdentity.LIdentityRecord(identity, card.LCardDraftId, row.LCollocationId);
        return row.LCollocationId;
    }

    public void LCardClerkSync(
        long ownerId, LCardDraft card, string language, bool collocation, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(identity);

        LSentenceSync(ownerId, card.LCardDraftSentence, language, collocation, identity);
        LSituationSync(ownerId, card.LCardDraftSituation, collocation, identity);
        _lCardClerkRegisters.LRegisterClerkSync(ownerId, card.LCardDraftRegister, language, collocation, identity);

        _lCardClerkTags.LTagClerkSave(ownerId, card.LCardDraftTag, collocation, identity);
        _lCardClerkTranslations.LTranslationClerkSave(ownerId, card.LCardDraftTranslation, collocation);

        LImageVault images = _lCardClerkImages;
        LCardClerkField.LCardFieldSync(
            LCardClerkField.LImageRead(card.LCardDraftImage),
            collocation ? images.LImageCollocationRead(ownerId) : images.LImageMeaningRead(ownerId),
            row => row.LImageId,
            written => LCardClerkField.LImageResolve(images, written, identity),
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

        LVideoVault videos = _lCardClerkVideos;
        LCardClerkField.LCardFieldSync(
            LCardClerkField.LVideoRead(card.LCardDraftVideo),
            collocation ? videos.LVideoCollocationRead(ownerId) : videos.LVideoMeaningRead(ownerId),
            row => row.LVideoId,
            written => LCardClerkField.LVideoResolve(videos, written, identity),
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

    private void LSentenceSync(
        long ownerId,
        IReadOnlyList<LSentenceDraft> drafts,
        string language,
        bool collocation,
        Dictionary<long, long> identity)
    {
        List<LSentence> rows = [];
        foreach (LSentenceDraft draft in LCardClerkField.LSentenceRead(drafts))
        {
            rows.Add(new LSentence(
                draft.LSentenceDraftId,
                ownerId,
                rows.Count,
                _lCardClerkExamples.LExampleClerkResolve(draft, language, ownerId, collocation, identity),
                draft.LSentenceDraftParticle,
                draft.LSentenceDraftDependence));
        }

        LSentenceVault sentences = _lCardClerkSentences;
        IReadOnlyList<long> written = collocation
            ? sentences.LSentenceCollocationSave(ownerId, rows)
            : sentences.LSentenceMeaningSave(ownerId, rows);

        for (int index = 0; index < rows.Count; index++)
        {
            LIdentity.LIdentityRecord(identity, rows[index].LSentenceId, written[index]);
        }
    }

    private void LSituationSync(
        long ownerId, IReadOnlyList<LSituationDraft> drafts, bool collocation, Dictionary<long, long> identity)
    {
        LSituationVault situations = _lCardClerkContexts;

        LCardClerkField.LCardFieldSync(
            LCardClerkField.LSituationRead(drafts),
            collocation ? situations.LSituationCollocationRead(ownerId) : situations.LSituationMeaningRead(ownerId),
            row => row.LSituationId,
            written => LCardClerkField.LSituationResolve(situations, written, identity),
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
}
