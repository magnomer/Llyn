using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineReflexApply(LEntryDraft content, LRequest request)
    {
        return request switch
        {
            LRequestReflexAddition sent => LEngineReflexAdd(content, sent),
            LRequestReflexRemoval sent => LEngineReflexApply(
                content,
                rows => LEngineListRemove(rows, sent.LRequestReflexId, static row => row.LReflexDraftId)),
            LRequestReflexLanguage sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => LEngineAnatomyResolve(
                    content.LEntryDraftLanguage,
                    LEngineRespellingResolve(row with { LReflexDraftLanguage = sent.LRequestText ?? string.Empty }))),
            LRequestReflexKind sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftKind = sent.LRequestText ?? string.Empty }),
            LRequestReflexText sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => LEngineAnatomyResolve(
                    content.LEntryDraftLanguage,
                    LEngineRespellingResolve(row with { LReflexDraftText = sent.LRequestText ?? string.Empty }))),
            LRequestReflexRespelling sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => LEngineAnatomyResolve(
                    content.LEntryDraftLanguage,
                    row with { LReflexDraftRespelling = sent.LRequestText ?? string.Empty })),
            LRequestReflexNote sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftNote = sent.LRequestText ?? string.Empty }),
            LRequestReflexRemark sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftRemark = sent.LRequestText ?? string.Empty }),
            LRequestReflexMain sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with { LReflexDraftMain = sent.LRequestMain }),
            LRequestReflexAnchor sent => LEngineReflexChange(
                content,
                sent.LRequestReflexId,
                row => row with
                {
                    LReflexDraftAnchors = LAnchor.LAnchorToggle(
                        row.LReflexDraftAnchors, sent.LRequestFanqieId, sent.LRequestAnchored),
                }),
            _ => LEngineListApply(content, request),
        };
    }

    private LEntryDraft LEngineReflexAdd(LEntryDraft content, LRequestReflexAddition request)
    {
        LReflexDraft row = new(
            (request.LRequestLanguage ?? string.Empty).Trim(),
            (request.LRequestKind ?? string.Empty).Trim(),
            LReflexDraftId: LEngineIdentityCreate());
        return LEngineReflexApply(content, rows => LEngineListAdd(rows, row, request.LRequestPosition));
    }

    private LReflexDraft LEngineRespellingResolve(LReflexDraft row)
    {
        string language = row.LReflexDraftLanguage.Trim();
        LLanguage? pack = language.Length == 0 ? null : LEngineLanguageLoad(language);
        if (pack is null || pack.LLanguageRespellings.Count == 0 || row.LReflexDraftText.Trim().Length == 0)
        {
            return row with { LReflexDraftRespelling = string.Empty };
        }

        return row with
        {
            LReflexDraftRespelling =
                LRespelling.LRespellingScan(pack.LLanguageRespellings, row.LReflexDraftText, string.Empty),
        };
    }

    private static LEntryDraft LEngineReflexChange(
        LEntryDraft content, long reflexId, Func<LReflexDraft, LReflexDraft> change)
    {
        IReadOnlyList<LReflexDraft> rows = LEngineListChange(
            content.LEntryDraftReflexes, reflexId, static row => row.LReflexDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return content with { LEntryDraftReflexes = rows };
    }

    private static LEntryDraft LEngineReflexApply(
        LEntryDraft content, Func<IReadOnlyList<LReflexDraft>, IReadOnlyList<LReflexDraft>> change)
    {
        return content with { LEntryDraftReflexes = change(content.LEntryDraftReflexes) };
    }
}
