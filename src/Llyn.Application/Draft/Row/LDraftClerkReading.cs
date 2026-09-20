using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkReading
{
    private readonly LLanguageCache _lDraftClerkLanguages;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkReading(LLanguageCache languages, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkLanguages = languages;
        _lDraftClerkIdentity = identity;
    }

    public LEntryDraft? LReadingApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestIpa sent => LPronunciationPrimaryChange(
                content,
                spoken => _lDraftClerkLanguages.LLanguageRespellingResolve(
                    content.LEntryDraftLanguage,
                    spoken with { LPronunciationDraftIpa = sent.LRequestText ?? string.Empty })),
            LRequestRespelling sent => LPronunciationPrimaryChange(
                content, spoken => spoken with { LPronunciationDraftRespelling = sent.LRequestText ?? string.Empty }),
            LRequestAudio sent => LPronunciationPrimaryChange(
                content,
                spoken => spoken with
                {
                    LPronunciationDraftAudio = sent.LRequestFile ?? string.Empty,
                    LPronunciationDraftSource = sent.LRequestSource,
                }),
            LRequestPronunciationAddition sent => LPronunciationAdd(content, sent),
            LRequestPronunciationRemoval sent => LPronunciationApply(
                content,
                spoken => LDraftClerkList.LDraftListRemove(
                    spoken, sent.LRequestPronunciationId, static row => row.LPronunciationDraftId)),
            LRequestPronunciationShift sent => LPronunciationApply(
                content,
                spoken => LDraftClerkList.LDraftListMove(
                    spoken,
                    sent.LRequestPronunciationId,
                    sent.LRequestPosition,
                    static row => row.LPronunciationDraftId)),
            LRequestPronunciationIpa sent => LPronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => _lDraftClerkLanguages.LLanguageRespellingResolve(
                    content.LEntryDraftLanguage,
                    spoken with { LPronunciationDraftIpa = sent.LRequestText ?? string.Empty })),
            LRequestPronunciationRespelling sent => LPronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => spoken with { LPronunciationDraftRespelling = sent.LRequestText ?? string.Empty }),
            LRequestPronunciationVariety sent => LPronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => _lDraftClerkLanguages.LLanguageRespellingResolve(
                    content.LEntryDraftLanguage,
                    spoken with { LPronunciationDraftVariety = sent.LRequestText ?? string.Empty })),
            LRequestPronunciationAudio sent => LPronunciationChange(
                content,
                sent.LRequestPronunciationId,
                spoken => spoken with
                {
                    LPronunciationDraftAudio = sent.LRequestFile ?? string.Empty,
                    LPronunciationDraftSource = sent.LRequestSource,
                }),
            LRequestTranscriptionAddition sent => LTranscriptionAdd(content, sent),
            LRequestTranscriptionRemoval sent => LTranscriptionApply(
                content,
                spelled => LDraftClerkList.LDraftListRemove(
                    spelled, sent.LRequestTranscriptionId, static row => row.LTranscriptionDraftId)),
            LRequestTranscriptionShift sent => LTranscriptionApply(
                content,
                spelled => LDraftClerkList.LDraftListMove(
                    spelled,
                    sent.LRequestTranscriptionId,
                    sent.LRequestPosition,
                    static row => row.LTranscriptionDraftId)),
            LRequestTranscriptionScheme sent => LSchemeChange(content, sent),
            LRequestTranscriptionText sent => LTranscriptionChange(
                content,
                sent.LRequestTranscriptionId,
                spelled => spelled with { LTranscriptionDraftText = sent.LRequestText ?? string.Empty }),
            _ => null,
        };
    }

    public static LEntryDraft LAudioClear(LEntryDraft content, LEntryDraft? stored)
    {
        ArgumentNullException.ThrowIfNull(content);

        List<LPronunciationDraft> spoken = new(content.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft row in content.LEntryDraftPronunciations)
        {
            spoken.Add(LAudioMatch(row, stored)
                ? row
                : row with { LPronunciationDraftAudio = string.Empty, LPronunciationDraftSource = null });
        }

        return content with { LEntryDraftPronunciations = spoken };
    }

    private static bool LAudioMatch(LPronunciationDraft row, LEntryDraft? stored)
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

    private LEntryDraft LPronunciationPrimaryChange(
        LEntryDraft content, Func<LPronunciationDraft, LPronunciationDraft> change)
    {
        if (content.LEntryDraftPronunciations.Count == 0)
        {
            LPronunciationDraft spoken = change(new LPronunciationDraft(
                string.Empty,
                LPronunciationDraftId: _lDraftClerkIdentity.LIdentityCreate(),
                LPronunciationDraftSeeded: true));
            return content with { LEntryDraftPronunciations = [spoken] };
        }

        return LPronunciationChange(content, content.LEntryDraftPronunciations[0].LPronunciationDraftId, change);
    }

    private LEntryDraft LPronunciationAdd(LEntryDraft content, LRequestPronunciationAddition request)
    {
        LPronunciationDraft spoken = _lDraftClerkLanguages.LLanguageRespellingResolve(
            content.LEntryDraftLanguage,
            new LPronunciationDraft(
                request.LRequestText ?? string.Empty,
                LPronunciationDraftId: _lDraftClerkIdentity.LIdentityCreate()));
        return LPronunciationApply(
            content, list => LDraftClerkList.LDraftListAdd(list, spoken, request.LRequestPosition));
    }

    private static LEntryDraft LPronunciationChange(
        LEntryDraft content, long pronunciationId, Func<LPronunciationDraft, LPronunciationDraft> change)
    {
        IReadOnlyList<LPronunciationDraft> spoken = LDraftClerkList.LDraftListChange(
            content.LEntryDraftPronunciations, pronunciationId, static row => row.LPronunciationDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return content with { LEntryDraftPronunciations = spoken };
    }

    private static LEntryDraft LPronunciationApply(
        LEntryDraft content, Func<IReadOnlyList<LPronunciationDraft>, IReadOnlyList<LPronunciationDraft>> change)
    {
        return content with { LEntryDraftPronunciations = change(content.LEntryDraftPronunciations) };
    }

    private LEntryDraft LTranscriptionAdd(LEntryDraft content, LRequestTranscriptionAddition request)
    {
        string scheme = (request.LRequestScheme ?? string.Empty).Trim();
        LSchemeValidate(content.LEntryDraftTranscriptions, scheme, 0);

        LTranscriptionDraft spelled = new(
            scheme,
            LTranscriptionDraftId: _lDraftClerkIdentity.LIdentityCreate(),
            LTranscriptionDraftSeeded: request.LRequestSeeded);
        return LTranscriptionApply(
            content, list => LDraftClerkList.LDraftListAdd(list, spelled, request.LRequestPosition));
    }

    private static LEntryDraft LSchemeChange(LEntryDraft content, LRequestTranscriptionScheme request)
    {
        string scheme = (request.LRequestText ?? string.Empty).Trim();
        LSchemeValidate(content.LEntryDraftTranscriptions, scheme, request.LRequestTranscriptionId);

        return LTranscriptionChange(
            content,
            request.LRequestTranscriptionId,
            spelled => spelled with { LTranscriptionDraftScheme = scheme });
    }

    private static void LSchemeValidate(IReadOnlyList<LTranscriptionDraft> drafts, string scheme, long ownId)
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

    private static LEntryDraft LTranscriptionChange(
        LEntryDraft content, long transcriptionId, Func<LTranscriptionDraft, LTranscriptionDraft> change)
    {
        IReadOnlyList<LTranscriptionDraft> spelled = LDraftClerkList.LDraftListChange(
            content.LEntryDraftTranscriptions, transcriptionId, static row => row.LTranscriptionDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return content with { LEntryDraftTranscriptions = spelled };
    }

    private static LEntryDraft LTranscriptionApply(
        LEntryDraft content, Func<IReadOnlyList<LTranscriptionDraft>, IReadOnlyList<LTranscriptionDraft>> change)
    {
        return content with { LEntryDraftTranscriptions = change(content.LEntryDraftTranscriptions) };
    }
}
