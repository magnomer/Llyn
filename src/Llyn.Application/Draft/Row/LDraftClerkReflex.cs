using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkReflex
{
    private readonly LLanguageCache _lDraftClerkLanguages;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkReflex(LLanguageCache languages, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkLanguages = languages;
        _lDraftClerkIdentity = identity;
    }

    public LEntryDraft? LReflexApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestReflexAddition sent => LReflexAdd(content, sent),
            LRequestReflexRemoval sent => LReflexApply(
                content,
                rows => LDraftClerkList.LDraftListRemove(
                    rows, sent.LRequestReflexId, static row => row.LReflexDraftId)),
            LRequestReflexLanguage sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => _lDraftClerkLanguages.LLanguageAnatomyResolve(
                    content.LEntryDraftLanguage,
                    _lDraftClerkLanguages.LLanguageRespellingResolve(
                        row with { LReflexDraftLanguage = sent.LRequestText ?? string.Empty }))),
            LRequestReflexKind sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftKind = sent.LRequestText ?? string.Empty }),
            LRequestReflexText sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => _lDraftClerkLanguages.LLanguageAnatomyResolve(
                    content.LEntryDraftLanguage,
                    _lDraftClerkLanguages.LLanguageRespellingResolve(
                        row with { LReflexDraftText = sent.LRequestText ?? string.Empty }))),
            LRequestReflexRespelling sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => _lDraftClerkLanguages.LLanguageAnatomyResolve(
                    content.LEntryDraftLanguage,
                    row with { LReflexDraftRespelling = sent.LRequestText ?? string.Empty })),
            LRequestReflexRomanization sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftRomanization = sent.LRequestText ?? string.Empty }),
            LRequestReflexMeaning sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with
                {
                    LReflexDraftMeaning = sent.LRequestText ?? string.Empty,
                    LReflexDraftOwned = true,
                }),
            LRequestReflexNote sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftNote = sent.LRequestText ?? string.Empty }),
            LRequestReflexMain sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftMain = sent.LRequestMain }),
            LRequestReflexAnchor sent => LReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with
                {
                    LReflexDraftAnchors = LAnchor.LAnchorToggle(
                        row.LReflexDraftAnchors, sent.LRequestFanqieId, sent.LRequestAnchored),
                }),
            _ => null,
        };
    }

    public static LReflexDraft? LReflexFind(LEntryDraft content, long id)
    {
        ArgumentNullException.ThrowIfNull(content);

        int index = LDraftClerkList.LDraftListFind(
            content.LEntryDraftReflexes, id, static row => row.LReflexDraftId);
        return index < 0 ? null : content.LEntryDraftReflexes[index];
    }

    public static LReflexDraft LReflexDefaultRead(LEntryDraft content, long reflex)
    {
        return reflex != 0 && LReflexFind(content, reflex) is LReflexDraft pressed
            ? new LReflexDraft(pressed.LReflexDraftLanguage, pressed.LReflexDraftKind)
            : new LReflexDraft(string.Empty);
    }

    public static int LReflexPositionRead(LEntryDraft content, long reflex)
    {
        ArgumentNullException.ThrowIfNull(content);

        int index = reflex == 0
            ? -1
            : LDraftClerkList.LDraftListFind(content.LEntryDraftReflexes, reflex, static row => row.LReflexDraftId);
        return index < 0 ? content.LEntryDraftReflexes.Count : index + 1;
    }

    private LEntryDraft LReflexAdd(LEntryDraft content, LRequestReflexAddition request)
    {
        LReflexDraft row = new(
            (request.LRequestLanguage ?? string.Empty).Trim(),
            (request.LRequestKind ?? string.Empty).Trim(),
            LReflexDraftId: _lDraftClerkIdentity.LIdentityCreate());
        return LReflexApply(content, rows => LDraftClerkList.LDraftListAdd(rows, row, request.LRequestPosition));
    }

    private static LEntryDraft LReflexChange(
        LEntryDraft content, long reflexId, Func<LReflexDraft, LReflexDraft> change)
    {
        IReadOnlyList<LReflexDraft> rows = LDraftClerkList.LDraftListChange(
            content.LEntryDraftReflexes, reflexId, static row => row.LReflexDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return content with { LEntryDraftReflexes = rows };
    }

    private static LEntryDraft LReflexApply(
        LEntryDraft content, Func<IReadOnlyList<LReflexDraft>, IReadOnlyList<LReflexDraft>> change)
    {
        return content with { LEntryDraftReflexes = change(content.LEntryDraftReflexes) };
    }
}
