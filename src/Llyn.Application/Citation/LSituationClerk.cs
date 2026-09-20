using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LSituationClerk
{
    private readonly LVault _lSituationClerkVault;
    private readonly LSituationVault _lSituationClerkSituations;
    private readonly LImageVault _lSituationClerkImages;
    private readonly LVideoVault _lSituationClerkVideos;

    public LSituationClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lSituationClerkVault = rig.LRigVault;
        _lSituationClerkSituations = rig.LRigSituations;
        _lSituationClerkImages = rig.LRigImages;
        _lSituationClerkVideos = rig.LRigVideos;
    }

    public static LSituation LSituationClerkBlank =>
        new(
            0,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

    public LSituation LSituationClerkCreate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);

        using LVaultSession session = _lSituationClerkVault.LVaultSessionStart();
        LSituation stored = _lSituationClerkSituations.LSituationCreate(situation);
        LSituationMediaSync(stored.LSituationId, situation);
        stored = _lSituationClerkSituations.LSituationRead(stored.LSituationId) ?? stored;
        session.LVaultSessionCommit();
        return stored;
    }

    public LSituation? LSituationClerkRead(long id)
    {
        return _lSituationClerkSituations.LSituationRead(id);
    }

    public IReadOnlyList<LSituation> LSituationClerkRead()
    {
        return _lSituationClerkSituations.LSituationRead();
    }

    public IReadOnlyList<LSituation> LSituationClerkRead(long ownerId, LOwner owner)
    {
        return LSituationOwnerCheck(owner)
            ? _lSituationClerkSituations.LSituationCollocationRead(ownerId)
            : _lSituationClerkSituations.LSituationMeaningRead(ownerId);
    }

    public LPortraitPage? LSituationClerkRead(long id, LPortraitLegend legend)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);

        LSituation? situation = _lSituationClerkSituations.LSituationRead(id);
        return situation is null
            ? null
            : LSituationPageRead(situation, _lSituationClerkSituations.LSituationReferenceRead(id), legend);
    }

    public IReadOnlyList<LCatalogSituation> LSituationClerkFind(string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        IReadOnlyList<LSituation> read = _lSituationClerkSituations.LSituationRead();
        IReadOnlyDictionary<long, int> usage = _lSituationClerkSituations.LSituationReferenceRead();

        List<LCatalogSituation> rows = [];
        foreach (LSituation situation in read)
        {
            usage.TryGetValue(situation.LSituationId, out int counted);

            LCatalogSituation row = LCatalogSituation.LCatalogSituationCreate(situation, counted);
            if (row.LCatalogSituationMatch(query))
            {
                rows.Add(row);
            }
        }

        return LCatalogSituation.LCatalogSituationSort(rows, order);
    }

    public void LSituationClerkUpdate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);

        using LVaultSession session = _lSituationClerkVault.LVaultSessionStart();
        _lSituationClerkSituations.LSituationUpdate(situation);
        LSituationMediaSync(situation.LSituationId, situation);
        session.LVaultSessionCommit();
    }

    public void LSituationClerkAttach(long ownerId, long situationId, int position, LOwner owner)
    {
        if (LSituationOwnerCheck(owner))
        {
            _lSituationClerkSituations.LSituationCollocationAttach(ownerId, situationId, position);
            return;
        }

        _lSituationClerkSituations.LSituationMeaningAttach(ownerId, situationId, position);
    }

    public void LSituationClerkDetach(long ownerId, long situationId, LOwner owner)
    {
        if (LSituationOwnerCheck(owner))
        {
            _lSituationClerkSituations.LSituationCollocationDetach(ownerId, situationId);
            return;
        }

        _lSituationClerkSituations.LSituationMeaningDetach(ownerId, situationId);
    }

    public void LSituationClerkRemove(long ownerId, long situationId, LOwner owner)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situationId);

        LSituationClerkDetach(ownerId, situationId, owner);
        if (_lSituationClerkSituations.LSituationReferenceRead(situationId) == 0)
        {
            _lSituationClerkSituations.LSituationDelete(situationId);
        }
    }

    public void LSituationClerkDelete(long id)
    {
        _lSituationClerkSituations.LSituationDelete(id);
    }

    public void LSituationClerkDelete(long id, bool detach)
    {
        _lSituationClerkSituations.LSituationDelete(id, detach);
    }

    public static bool LSituationClerkMatch(LSituation one, LSituation other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        return one.LSituationTitle == other.LSituationTitle
            && one.LSituationDescription == other.LSituationDescription
            && one.LSituationKind == other.LSituationKind
            && LDraftClerkEquality.LImageMatch(one.LSituationImage, other.LSituationImage)
            && LDraftClerkEquality.LVideoMatch(one.LSituationVideo, other.LSituationVideo);
    }

    public static LPortraitPage LSituationPageRead(LSituation situation, int count, LPortraitLegend legend)
    {
        ArgumentNullException.ThrowIfNull(situation);
        ArgumentNullException.ThrowIfNull(legend);

        string mark = legend.LPortraitLegendUnknown;
        List<string> chips = [];

        string kind = LPortraitText.LPortraitTextRead(situation.LSituationKind, mark);
        if (kind.Length > 0)
        {
            chips.Add(kind);
        }

        chips.Add(legend.LPortraitTallyFormat(count));

        List<LPortraitSection> sections = [];

        string description = LPortraitText.LPortraitTextRead(situation.LSituationDescription, mark);
        if (description.Length > 0)
        {
            sections.Add(new LPortraitSection(legend.LPortraitLegendDescription, [], description, [], []));
        }

        IReadOnlyList<LPortraitMedia> images = LPortraitMedia.LPortraitMediaCreate(situation.LSituationImage, mark);
        IReadOnlyList<LPortraitMedia> videos = LPortraitMedia.LPortraitMediaCreate(situation.LSituationVideo, mark);
        if (images.Count > 0 || videos.Count > 0)
        {
            sections.Add(new LPortraitSection(string.Empty, [], string.Empty, images, videos));
        }

        return new LPortraitPage(
            LPortraitText.LPortraitTitleRead(situation.LSituationTitle, legend.LPortraitLegendUntitled, mark),
            string.Empty,
            chips,
            sections);
    }

    private void LSituationMediaSync(long situationId, LSituation situation)
    {
        Dictionary<long, long> identity = [];

        LImageVault images = _lSituationClerkImages;
        LCardClerkField.LCardFieldSync(
            LCardClerkField.LImageRead(situation.LSituationImage),
            images.LImageSituationRead(situationId),
            row => row.LImageId,
            written => LCardClerkField.LImageResolve(images, written, identity),
            rowId => images.LImageSituationDetach(situationId, rowId),
            (rowId, position) => images.LImageSituationAttach(situationId, rowId, position));

        LVideoVault videos = _lSituationClerkVideos;
        LCardClerkField.LCardFieldSync(
            LCardClerkField.LVideoRead(situation.LSituationVideo),
            videos.LVideoSituationRead(situationId),
            row => row.LVideoId,
            written => LCardClerkField.LVideoResolve(videos, written, identity),
            rowId => videos.LVideoSituationDetach(situationId, rowId),
            (rowId, position) => videos.LVideoSituationAttach(situationId, rowId, position));
    }

    private static bool LSituationOwnerCheck(LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerMeaning => false,
            LOwner.LOwnerCollocation => true,
            _ => throw new ArgumentOutOfRangeException(
                nameof(owner), owner, "This entity has no reference from that kind of row."),
        };
    }
}
