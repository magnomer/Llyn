using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkEntry
{
    private readonly LLanguageCache _lDraftClerkLanguages;

    public LDraftClerkEntry(LLanguageCache languages)
    {
        ArgumentNullException.ThrowIfNull(languages);
        _lDraftClerkLanguages = languages;
    }

    public LEntryDraft? LEntryApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestHeadword sent => content with { LEntryDraftHeadword = sent.LRequestText ?? string.Empty },
            LRequestLanguage sent => _lDraftClerkLanguages.LLanguageUnitRebuild(
                _lDraftClerkLanguages.LLanguageAnatomyRebuild(
                    _lDraftClerkLanguages.LLanguageRespellingRebuild(
                        content with { LEntryDraftLanguage = sent.LRequestText ?? string.Empty }))),
            LRequestNote sent => content with { LEntryDraftNote = sent.LRequestText ?? string.Empty },
            LRequestSpeech sent => content with { LEntryDraftSpeeches = sent.LRequestSpeeches ?? [] },
            LRequestUnit sent => content with { LEntryDraftUnit = sent.LRequestValue },
            _ => null,
        };
    }

    public static string LNoteResolve(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text.TrimEnd('\r', '\n');
    }

    public static bool LNoteCheck(string text, string note)
    {
        return string.Equals(LNoteResolve(text), note, StringComparison.Ordinal);
    }
}
