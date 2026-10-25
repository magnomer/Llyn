using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillExample
{
    private readonly LTenure _lQuillExampleTenure;

    private readonly LDraftPort _lQuillExampleDrafts;

    private readonly LQuillChip _lQuillExampleChip;

    public LQuillExample(LTenure tenure, LDraftPort drafts)
    {
        ArgumentNullException.ThrowIfNull(tenure);
        ArgumentNullException.ThrowIfNull(drafts);

        _lQuillExampleTenure = tenure;
        _lQuillExampleDrafts = drafts;
        _lQuillExampleChip = new LQuillChip(tenure, drafts);
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

    public LReferenceOffer LQuillReferenceFind(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return _lQuillExampleDrafts.LEngineReferenceFind(_lQuillExampleTenure, new LExampleSiteDraft(), text);
    }

    public void LQuillReferenceResolve(string title)
    {
        ArgumentNullException.ThrowIfNull(title);

        long reference = _lQuillExampleDrafts.LEngineCitationResolve(
            _lQuillExampleTenure, new LExampleSiteDraft(), title);
        LExampleReferenceSet(reference);
    }

    public void LQuillMentionAdd(string text, int start, int length, long entry)
    {
        _lQuillExampleChip.LQuillMentionAdd(new LExampleSiteDraft(), text, start, length, entry);
    }

    public void LQuillSenseSet(string text, int start, int length, long sense)
    {
        _lQuillExampleChip.LQuillSenseSet(new LExampleSiteDraft(), text, start, length, sense);
    }

    public void LQuillMentionRemove(string text, int start, int length)
    {
        _lQuillExampleChip.LQuillMentionRemove(new LExampleSiteDraft(), text, start, length);
    }
}
