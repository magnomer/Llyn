using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTranscript
{
    private readonly CDesk _cTranscriptDesk;

    private readonly CAnthology _cTranscriptAnthology;

    private readonly LDraftPort _cTranscriptDraftPort;

    private readonly LSettingsPort _cTranscriptSettingsPort;

    private readonly CEnvoy _cTranscriptEnvoy;

    internal CTranscript(CDesk desk, CAnthology anthology, LDraftPort drafts, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(anthology);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cTranscriptDesk = desk;
        _cTranscriptAnthology = anthology;
        _cTranscriptDraftPort = drafts;
        _cTranscriptSettingsPort = settings;
        _cTranscriptEnvoy = envoy;
    }

    public event Action<CExample>? CTranscriptDraftChanged;

    public event Action<CProspect>? CTranscriptMentionOffered;

    internal void LTranscriptObserverAttach(Action<Action> marshal)
    {
        _cTranscriptDesk.CDeskObserverAttach(marshal, LTranscriptDraftResonate);
    }

    internal CExample? LTranscriptRead()
    {
        try
        {
            return _cTranscriptAnthology.LAnthologyDraftRead(_cTranscriptDesk.CDeskDraft.CDeskDraftRead());
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cTranscriptEnvoy, _cTranscriptSettingsPort, "Example.HoldFailed", exception);
            return null;
        }
    }

    private void LTranscriptDraftResonate()
    {
        if (LTranscriptRead() is CExample example)
        {
            CTranscriptDraftChanged?.Invoke(_cTranscriptAnthology.LAnthologyTranscriptRead(example));
        }
    }

    public void CTranscriptMentionOpen(string word)
    {
        CTranscriptMentionOffered?.Invoke(
            CMention.LMentionProspectRead(_cTranscriptDesk, word, _cTranscriptEnvoy, _cTranscriptSettingsPort));
    }

    public void CTranscriptMentionAdd(string text, int start, int length, long? entryId)
    {
        if (entryId is not long stored)
        {
            _cTranscriptEnvoy.CEnvoyFailureShow("Refusal.TargetMissing");
            return;
        }

        _cTranscriptDesk.CDeskDraft.CDeskDraftExample?.LQuillMentionAdd(text, start, length, stored);
    }

    public void CTranscriptSilenceSet(string text, int start, int length)
    {
        _cTranscriptDesk.CDeskDraft.CDeskDraftExample?.LQuillMentionAdd(text, start, length, 0);
    }

    public void CTranscriptSenseSet(string text, int start, int length, long senseId)
    {
        _cTranscriptDesk.CDeskDraft.CDeskDraftExample?.LQuillSenseSet(text, start, length, senseId);
    }

    public void CTranscriptMentionRemove(long mentionId)
    {
        _cTranscriptDesk.CDeskDraft.CDeskDraftMention?.LQuillMentionRemove(mentionId);
    }

    public void CTranscriptMentionRemove(string text, int start, int length)
    {
        _cTranscriptDesk.CDeskDraft.CDeskDraftExample?.LQuillMentionRemove(text, start, length);
    }

    public bool CTranscriptMentionCheck(string text, int start, int length)
    {
        return _cTranscriptDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            && new LQuillMention(held).LQuillMentionCheck(text, start, length);
    }

    public bool CTranscriptSenseCheck(string text, int start, int length)
    {
        return _cTranscriptDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            && new LQuillMention(held).LQuillSenseCheck(text, start, length);
    }

    public CMentionSense? CTranscriptSenseRead(string text, int start, int length)
    {
        return CMention.LMentionSenseRead(
            _cTranscriptDraftPort,
            _cTranscriptDesk,
            new LExampleSiteDraft(),
            text,
            start,
            length,
            _cTranscriptEnvoy,
            _cTranscriptSettingsPort);
    }

    public IReadOnlyList<CMentionLabel> CTranscriptMentionRead()
    {
        return CMention.LMentionChipRead(
            _cTranscriptDraftPort,
            _cTranscriptDesk,
            new LExampleSiteDraft(),
            _cTranscriptEnvoy,
            _cTranscriptSettingsPort);
    }
}
