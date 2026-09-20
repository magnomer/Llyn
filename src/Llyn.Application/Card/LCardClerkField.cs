using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LCardClerkField
{
    public static void LCardValidate(IReadOnlyList<LCardDraft> cards, bool collocation)
    {
        ArgumentNullException.ThrowIfNull(cards);

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

    public static IEnumerable<LCardDraft> LCardRead(IReadOnlyList<LCardDraft> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        foreach (LCardDraft card in cards)
        {
            if (!card.LCardDraftEmpty)
            {
                yield return card;
            }
        }
    }

    public static IEnumerable<LSentenceDraft> LSentenceRead(IReadOnlyList<LSentenceDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        foreach (LSentenceDraft draft in drafts)
        {
            if (!draft.LSentenceDraftEmpty)
            {
                yield return draft;
            }
        }
    }

    public static IEnumerable<LSituationDraft> LSituationRead(IReadOnlyList<LSituationDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        foreach (LSituationDraft draft in drafts)
        {
            if (!draft.LSituationDraftTitle.LStateValueEmpty)
            {
                yield return draft;
            }
        }
    }

    public static long LSituationResolve(
        LSituationVault situations, LSituationDraft draft, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(draft);

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

        LSituation? found = LDraftClerkChip.LSituationResolve(situations, draft.LSituationDraftTitle);
        if (found is not null)
        {
            return found.LSituationId;
        }

        long created = situations.LSituationCreate(new LSituation(
            0,
            draft.LSituationDraftTitle,
            draft.LSituationDraftDescription,
            draft.LSituationDraftKind)).LSituationId;
        LIdentity.LIdentityRecord(identity, draft.LSituationDraftId, created);
        return created;
    }

    public static IEnumerable<LVideoDraft> LVideoRead(IReadOnlyList<LVideoDraft> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        foreach (LVideoDraft row in rows)
        {
            if (!row.LVideoDraftEmpty)
            {
                yield return row;
            }
        }
    }

    public static IEnumerable<LImageDraft> LImageRead(IReadOnlyList<LImageDraft> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        foreach (LImageDraft row in rows)
        {
            if (!row.LImageDraftEmpty)
            {
                yield return row;
            }
        }
    }

    public static long LImageResolve(LImageVault images, LImageDraft draft, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(draft);

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
        LIdentity.LIdentityRecord(identity, draft.LImageDraftId, created);
        return created;
    }

    public static long LVideoResolve(LVideoVault videos, LVideoDraft draft, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(videos);
        ArgumentNullException.ThrowIfNull(draft);

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
        LIdentity.LIdentityRecord(identity, draft.LVideoDraftId, created);
        return created;
    }

    public static void LCardFieldSync<LCardRow, LCardWritten>(
        IEnumerable<LCardWritten> written,
        IReadOnlyList<LCardRow> attached,
        Func<LCardRow, long> identify,
        Func<LCardWritten, long> resolve,
        Action<long> detach,
        Action<long, int> attach)
    {
        ArgumentNullException.ThrowIfNull(written);
        ArgumentNullException.ThrowIfNull(attached);
        ArgumentNullException.ThrowIfNull(identify);
        ArgumentNullException.ThrowIfNull(resolve);
        ArgumentNullException.ThrowIfNull(detach);
        ArgumentNullException.ThrowIfNull(attach);

        List<long> targets = [];
        HashSet<long> kept = [];
        foreach (LCardWritten text in written)
        {
            long settled = resolve(text);
            if (!kept.Add(settled))
            {
                continue;
            }

            targets.Add(settled);
        }

        foreach (LCardRow row in attached)
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
