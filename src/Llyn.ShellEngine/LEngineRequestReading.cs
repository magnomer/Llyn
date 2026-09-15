using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineReadingApply(LEntryDraft content, LRequest request)
    {
        return request switch
        {
            LRequestIpa sent => LEnginePrimaryChange(
                content, spoken => spoken with { LPronunciationDraftIpa = sent.LRequestText ?? string.Empty }),
            LRequestAudio sent => LEnginePrimaryChange(
                content,
                spoken => spoken with
                {
                    LPronunciationDraftAudio = sent.LRequestFile ?? string.Empty,
                    LPronunciationDraftSource = sent.LRequestSource,
                }),
            LRequestPronunciationAddition sent => LEnginePronunciationAdd(content, sent),
            LRequestPronunciationRemoval sent => LEnginePronunciationApply(
                content,
                spoken => LEngineListRemove(
                    spoken, sent.LRequestPronunciationId, static row => row.LPronunciationDraftId)),
            LRequestPronunciationShift sent => LEnginePronunciationApply(
                content,
                spoken => LEngineListMove(
                    spoken,
                    sent.LRequestPronunciationId,
                    sent.LRequestPosition,
                    static row => row.LPronunciationDraftId)),
            LRequestPronunciationIpa sent => LEnginePronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => spoken with { LPronunciationDraftIpa = sent.LRequestText ?? string.Empty }),
            LRequestPronunciationVariety sent => LEnginePronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => spoken with { LPronunciationDraftVariety = sent.LRequestText ?? string.Empty }),
            LRequestPronunciationAudio sent => LEnginePronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => spoken with
                {
                    LPronunciationDraftAudio = sent.LRequestFile ?? string.Empty,
                    LPronunciationDraftSource = sent.LRequestSource,
                }),
            LRequestTranscriptionAddition sent => LEngineTranscriptionAdd(content, sent),
            LRequestTranscriptionRemoval sent => LEngineTranscriptionApply(
                content,
                spelled => LEngineListRemove(
                    spelled, sent.LRequestTranscriptionId, static row => row.LTranscriptionDraftId)),
            LRequestTranscriptionShift sent => LEngineTranscriptionApply(
                content,
                spelled => LEngineListMove(
                    spelled,
                    sent.LRequestTranscriptionId,
                    sent.LRequestPosition,
                    static row => row.LTranscriptionDraftId)),
            LRequestTranscriptionScheme sent => LEngineSchemeChange(content, sent),
            LRequestTranscriptionText sent => LEngineTranscriptionChange(
                content,
                sent.LRequestTranscriptionId,
                spelled => spelled with { LTranscriptionDraftText = sent.LRequestText ?? string.Empty }),
            _ => LEngineListApply(content, request),
        };
    }

    private LEntryDraft LEnginePrimaryChange(
        LEntryDraft content, Func<LPronunciationDraft, LPronunciationDraft> change)
    {
        if (content.LEntryDraftPronunciations.Count == 0)
        {
            LPronunciationDraft spoken = change(new LPronunciationDraft(
                string.Empty, LPronunciationDraftId: LEngineIdentityCreate(), LPronunciationDraftSeeded: true));
            return content with { LEntryDraftPronunciations = [spoken] };
        }

        return LEnginePronunciationChange(content, content.LEntryDraftPronunciations[0].LPronunciationDraftId, change);
    }

    private LEntryDraft LEnginePronunciationAdd(LEntryDraft content, LRequestPronunciationAddition request)
    {
        LPronunciationDraft spoken = new(
            request.LRequestText ?? string.Empty, LPronunciationDraftId: LEngineIdentityCreate());
        return LEnginePronunciationApply(content, list => LEngineListAdd(list, spoken, request.LRequestPosition));
    }

    private static LEntryDraft LEnginePronunciationChange(
        LEntryDraft content, long pronunciationId, Func<LPronunciationDraft, LPronunciationDraft> change)
    {
        IReadOnlyList<LPronunciationDraft> spoken = LEngineListChange(
            content.LEntryDraftPronunciations, pronunciationId, static row => row.LPronunciationDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return content with { LEntryDraftPronunciations = spoken };
    }

    private static LEntryDraft LEnginePronunciationApply(
        LEntryDraft content, Func<IReadOnlyList<LPronunciationDraft>, IReadOnlyList<LPronunciationDraft>> change)
    {
        return content with { LEntryDraftPronunciations = change(content.LEntryDraftPronunciations) };
    }

    private LEntryDraft LEngineTranscriptionAdd(LEntryDraft content, LRequestTranscriptionAddition request)
    {
        string scheme = (request.LRequestScheme ?? string.Empty).Trim();
        LEngineSchemeValidate(content.LEntryDraftTranscriptions, scheme, 0);

        LTranscriptionDraft spelled = new(
            scheme, LTranscriptionDraftId: LEngineIdentityCreate(), LTranscriptionDraftSeeded: request.LRequestSeeded);
        return LEngineTranscriptionApply(content, list => LEngineListAdd(list, spelled, request.LRequestPosition));
    }

    private static LEntryDraft LEngineSchemeChange(LEntryDraft content, LRequestTranscriptionScheme request)
    {
        string scheme = (request.LRequestText ?? string.Empty).Trim();
        LEngineSchemeValidate(content.LEntryDraftTranscriptions, scheme, request.LRequestTranscriptionId);

        return LEngineTranscriptionChange(
            content,
            request.LRequestTranscriptionId,
            spelled => spelled with { LTranscriptionDraftScheme = scheme });
    }

    private static void LEngineSchemeValidate(IReadOnlyList<LTranscriptionDraft> drafts, string scheme, long ownId)
    {
        if (scheme.Length == 0)
        {
            return;
        }

        foreach (LTranscriptionDraft draft in drafts)
        {
            if (draft.LTranscriptionDraftId != ownId
                && string.Equals(draft.LTranscriptionDraftScheme.Trim(), scheme, StringComparison.Ordinal))
            {
                throw new LRefusal(LRefusal.LRefusalScheme);
            }
        }
    }

    private static LEntryDraft LEngineTranscriptionChange(
        LEntryDraft content, long transcriptionId, Func<LTranscriptionDraft, LTranscriptionDraft> change)
    {
        IReadOnlyList<LTranscriptionDraft> spelled = LEngineListChange(
            content.LEntryDraftTranscriptions, transcriptionId, static row => row.LTranscriptionDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return content with { LEntryDraftTranscriptions = spelled };
    }

    private static LEntryDraft LEngineTranscriptionApply(
        LEntryDraft content, Func<IReadOnlyList<LTranscriptionDraft>, IReadOnlyList<LTranscriptionDraft>> change)
    {
        return content with { LEntryDraftTranscriptions = change(content.LEntryDraftTranscriptions) };
    }
}
