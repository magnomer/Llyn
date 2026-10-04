using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkChip
{
    private readonly LTagVault _lDraftClerkTags;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkChip(LTagVault tags, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkTags = tags;
        _lDraftClerkIdentity = identity;
    }

    public LEntryDraft LTagAdd(LEntryDraft content, LRequestTagAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string text = (request.LRequestText ?? string.Empty).Trim();
        if (text.Length == 0)
        {
            return content;
        }

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftHeldCheck(tags, static row => row.LTagDraftText, text)
                ? tags
                : LDraftClerkList.LDraftListAdd(
                    tags, new LTagDraft(_lDraftClerkIdentity.LIdentityCreate(), text), request.LRequestPosition));
    }

    public LEntryDraft LTagInsert(LEntryDraft content, LRequestTagPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestTagId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LTag stored = _lDraftClerkTags.LTagRead(request.LRequestTagId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LTagDraft tag = new(stored.LTagId, stored.LTagText);

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftHeldCheck(tags, static row => row.LTagDraftText, stored.LTagText)
                ? tags
                : LDraftClerkList.LDraftListInsert(
                    tags, tag, stored.LTagId, request.LRequestPosition, static row => row.LTagDraftId));
    }

    public static LEntryDraft LTagRemove(LEntryDraft content, LRequestTagRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftListRemove(tags, request.LRequestTagId, static row => row.LTagDraftId));
    }

    public static LEntryDraft LTagMove(LEntryDraft content, LRequestTagShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftListMove(
                tags, request.LRequestTagId, request.LRequestPosition, static row => row.LTagDraftId));
    }

    public static LEntryDraft LTagChange(LEntryDraft content, LRequestTagText request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string text = (request.LRequestText ?? string.Empty).Trim();

        return LDraftClerkList.LDraftRowChange(content, card =>
        {
            IReadOnlyList<LTagDraft>? tags = LDraftClerkList.LDraftListChange(
                card.LCardDraftTag,
                request.LRequestTagId,
                static row => row.LTagDraftId,
                tag => tag with { LTagDraftText = text });

            return tags is null ? null : card with { LCardDraftTag = tags };
        });
    }

    private static LEntryDraft LTagApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<LTagDraft>, IReadOnlyList<LTagDraft>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftTag = change(card.LCardDraftTag) });
    }

    public static LEntryDraft LTranslationInsert(LEntryDraft content, LRequestTranslationPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestEntryId == 0)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }

        return LTranslationApply(
            content,
            request.LRequestCardId,
            translations => LDraftClerkList.LDraftListInsert(
                translations,
                request.LRequestEntryId,
                request.LRequestEntryId,
                request.LRequestPosition,
                static row => row));
    }

    public static LEntryDraft LTranslationRemove(LEntryDraft content, LRequestTranslationRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTranslationApply(
            content,
            request.LRequestCardId,
            translations => LDraftClerkList.LDraftListRemove(translations, request.LRequestEntryId, static row => row));
    }

    public static LEntryDraft LTranslationMove(LEntryDraft content, LRequestTranslationShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTranslationApply(
            content,
            request.LRequestCardId,
            translations => LDraftClerkList.LDraftListMove(
                translations, request.LRequestEntryId, request.LRequestPosition, static row => row));
    }

    private static LEntryDraft LTranslationApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<long>, IReadOnlyList<long>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftTranslation = change(card.LCardDraftTranslation) });
    }
}
