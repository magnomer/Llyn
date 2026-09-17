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
                content,
                spoken => LEngineRespellingResolve(
                    content.LEntryDraftLanguage,
                    spoken with { LPronunciationDraftIpa = sent.LRequestText ?? string.Empty })),
            LRequestRespelling sent => LEnginePrimaryChange(
                content, spoken => spoken with { LPronunciationDraftRespelling = sent.LRequestText ?? string.Empty }),
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
                spoken => LEngineRespellingResolve(
                    content.LEntryDraftLanguage,
                    spoken with { LPronunciationDraftIpa = sent.LRequestText ?? string.Empty })),
            LRequestPronunciationRespelling sent => LEnginePronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => spoken with { LPronunciationDraftRespelling = sent.LRequestText ?? string.Empty }),
            LRequestPronunciationVariety sent => LEnginePronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => LEngineRespellingResolve(
                    content.LEntryDraftLanguage,
                    spoken with { LPronunciationDraftVariety = sent.LRequestText ?? string.Empty })),
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
            _ => LEngineReflexApply(content, request),
        };
    }

    private LDraft LEngineAudioClear(LDraft draft, LRequest request)
    {
        LEntryDraft before = draft.LDraftContent;
        LEntryDraft content = LEngineRequestApply(before, request);
        if (!string.Equals(before.LEntryDraftLanguage, content.LEntryDraftLanguage, StringComparison.Ordinal))
        {
            content = LEngineAudioClear(content, null);
        }
        else if (!string.Equals(before.LEntryDraftHeadword, content.LEntryDraftHeadword, StringComparison.Ordinal))
        {
            LEntryDraft? stored = draft.LDraftEntryId <= 0 ? null : LEngineEntryLoad(draft.LDraftEntryId);
            content = LEngineAudioClear(content, stored);
        }

        return draft with { LDraftContent = content };
    }

    private static LEntryDraft LEngineAudioClear(LEntryDraft content, LEntryDraft? stored)
    {
        List<LPronunciationDraft> spoken = new(content.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft row in content.LEntryDraftPronunciations)
        {
            spoken.Add(LEngineAudioMatch(row, stored)
                ? row
                : row with { LPronunciationDraftAudio = string.Empty, LPronunciationDraftSource = null });
        }

        return content with { LEntryDraftPronunciations = spoken };
    }

    private static bool LEngineAudioMatch(LPronunciationDraft row, LEntryDraft? stored)
    {
        if (row.LPronunciationDraftAudio.Length == 0)
        {
            return true;
        }

        if (stored is null)
        {
            return false;
        }

        foreach (LPronunciationDraft kept in stored.LEntryDraftPronunciations)
        {
            if (kept.LPronunciationDraftId == row.LPronunciationDraftId)
            {
                return string.Equals(
                    kept.LPronunciationDraftAudio, row.LPronunciationDraftAudio, StringComparison.Ordinal);
            }
        }

        return false;
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
        LPronunciationDraft spoken = LEngineRespellingResolve(
            content.LEntryDraftLanguage,
            new LPronunciationDraft(
                request.LRequestText ?? string.Empty, LPronunciationDraftId: LEngineIdentityCreate()));
        return LEnginePronunciationApply(content, list => LEngineListAdd(list, spoken, request.LRequestPosition));
    }

    private LPronunciationDraft LEngineRespellingResolve(string language, LPronunciationDraft spoken)
    {
        IReadOnlyList<LRespelling> groups = language.Trim().Length == 0
            ? []
            : LEngineLanguageLoad(language).LLanguageRespellings;
        string respelling = groups.Count == 0 || spoken.LPronunciationDraftIpa.Length == 0
            ? string.Empty
            : LRespelling.LRespellingScan(
                groups, spoken.LPronunciationDraftIpa, spoken.LPronunciationDraftVariety.Trim());
        return spoken with { LPronunciationDraftRespelling = respelling };
    }

    private LEntryDraft LEngineRespellingRebuild(LEntryDraft content)
    {
        List<LPronunciationDraft> spoken = new(content.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft row in content.LEntryDraftPronunciations)
        {
            spoken.Add(LEngineRespellingResolve(content.LEntryDraftLanguage, row));
        }

        return content with { LEntryDraftPronunciations = spoken };
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
