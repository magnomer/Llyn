using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillSpeech
{
    private readonly LTenure _lQuillSpeechTenure;

    private string? _lQuillSpeechPending;

    public LQuillSpeech(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillSpeechTenure = tenure;
    }

    public (IReadOnlyList<string> LSpeechNames, string LSpeechTyped, LSpeechOffer LSpeechFound) LQuillSpeechRead(
        string typed)
    {
        IReadOnlyList<LSpeechDraft> shown = _lQuillSpeechTenure.LTenureRead()?.LDraftContent.LEntryDraftSpeeches ?? [];
        (IReadOnlyList<LSpeechDraft> held, string kept) =
            LSpeechClerk.LSpeechSettle(shown, LSpeechClerk.LSpeechChipRead(shown, _lQuillSpeechPending), typed);
        if (kept.Length == 0)
        {
            _lQuillSpeechPending = null;
        }

        return (held.Select(static speech => speech.LSpeechDraftName).ToList(), kept, LQuillSpeechFind(held, kept));
    }

    public LSpeechOffer LQuillSpeechSet(string typed)
    {
        IReadOnlyList<LSpeechDraft> held = LQuillSpeechScan();
        LQuillSpeechSend(held, typed, true);
        return LQuillSpeechFind(held, typed);
    }

    public void LQuillSpeechAdd(string name)
    {
        string language = _lQuillSpeechTenure.LTenureLanguageRead();
        IReadOnlyList<LSpeechDraft>? added =
            LSpeechClerk.LSpeechAdd(LQuillSpeechScan(), name, typed => LQuillSpeechCreate(language, typed));
        if (added is null)
        {
            return;
        }

        LQuillSpeechSend(added, string.Empty, false);
    }

    public void LQuillSpeechRemove(string name, string typed)
    {
        LQuillSpeechSend(LSpeechClerk.LSpeechRemove(LQuillSpeechScan(), name), typed, false);
    }

    private IReadOnlyList<LSpeechDraft> LQuillSpeechScan()
    {
        IReadOnlyList<LSpeechDraft> shown = _lQuillSpeechTenure.LTenureRead()?.LDraftContent.LEntryDraftSpeeches ?? [];
        return LSpeechClerk.LSpeechChipRead(shown, _lQuillSpeechPending);
    }

    private void LQuillSpeechSend(IReadOnlyList<LSpeechDraft> held, string typed, bool deferred)
    {
        _lQuillSpeechPending = LSpeechClerk.LSpeechPendingRead(held, typed);
        LRequestSpeech request = new(_lQuillSpeechTenure.LTenureId, LSpeechClerk.LSpeechParse(held, typed));
        if (deferred)
        {
            _lQuillSpeechTenure.LTenureRequestDefer(request);
            return;
        }

        _lQuillSpeechTenure.LTenureRequestApply(request);
    }

    private LSpeechOffer LQuillSpeechFind(IReadOnlyList<LSpeechDraft> held, string typed)
    {
        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _lQuillSpeechTenure.LTenureEngine.LEngineVocabulary.LEngineSpeechRead(
                _lQuillSpeechTenure.LTenureLanguageRead());
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            values = [];
        }

        return LSpeechClerk.LSpeechFind(values, held, typed);
    }

    private LSpeechValue? LQuillSpeechCreate(string language, string name)
    {
        try
        {
            return _lQuillSpeechTenure.LTenureEngine.LEngineVocabulary.LEngineSpeechAdd(language, name);
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            return null;
        }
    }
}
