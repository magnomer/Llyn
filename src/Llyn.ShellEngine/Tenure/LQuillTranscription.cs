using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillTranscription
{
    private readonly LTenure _lQuillTranscriptionTenure;

    public LQuillTranscription(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillTranscriptionTenure = tenure;
    }

    public LTranscriptionSheet? LQuillTranscriptionRead()
    {
        LEngine engine = _lQuillTranscriptionTenure.LTenureEngine;
        return _lQuillTranscriptionTenure.LTenureRead()?.LDraftContent is LEntryDraft content
            ? LDraftClerkReading.LTranscriptionSheetRead(
                engine.LEnginePronunciation.LEngineSchemeRead(content.LEntryDraftLanguage),
                content.LEntryDraftTranscriptions,
                engine.LEngineLanguage.LEngineGlyphRead(content).LGlyphBlockOther)
            : null;
    }

    public void LQuillTranscriptionSet(long transcription, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTranscriptionTenure.LTenureRequestDefer(
            new LRequestTranscriptionText(_lQuillTranscriptionTenure.LTenureId, transcription, text));
    }

    public void LTranscriptionSchemeSet(long transcription, string scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        _lQuillTranscriptionTenure.LTenureRequestApply(
            new LRequestTranscriptionScheme(_lQuillTranscriptionTenure.LTenureId, transcription, scheme));
    }

    public void LQuillTranscriptionAdd(long transcription)
    {
        if (LQuillTranscriptionRead()?.LTranscriptionSheetScheme is not string scheme)
        {
            return;
        }

        IReadOnlyList<LTranscriptionDraft> spelled =
            _lQuillTranscriptionTenure.LTenureRead()?.LDraftContent.LEntryDraftTranscriptions ?? [];
        _lQuillTranscriptionTenure.LTenureRequestApply(new LRequestTranscriptionAddition(
            _lQuillTranscriptionTenure.LTenureId,
            scheme,
            LDraftClerkReading.LTranscriptionPositionRead(spelled, transcription)));
    }

    public void LQuillTranscriptionRemove(long transcription)
    {
        _lQuillTranscriptionTenure.LTenureRequestApply(
            new LRequestTranscriptionRemoval(_lQuillTranscriptionTenure.LTenureId, transcription));
    }
}
