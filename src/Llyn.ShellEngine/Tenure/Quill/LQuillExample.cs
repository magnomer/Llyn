using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillExample
{
    private readonly LTenure _lQuillExampleTenure;

    public LQuillExample(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillExampleTenure = tenure;
    }

    public void LQuillExampleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillExampleTenure.LTenureRequestDefer(
            new LRequestExampleText(_lQuillExampleTenure.LTenureId, new LStateWritten(text, false)));
    }

    public void LExampleSpeakerSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        _lQuillExampleTenure.LTenureRequestApply(
            new LRequestExampleLanguage(_lQuillExampleTenure.LTenureId, language));
    }

    public void LExampleReferenceSet(long reference)
    {
        _lQuillExampleTenure.LTenureRequestApply(
            new LRequestExampleReference(_lQuillExampleTenure.LTenureId, reference));
    }
}
