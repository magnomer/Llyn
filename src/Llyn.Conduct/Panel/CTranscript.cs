using System;
using System.Collections.Generic;
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
            return _cTranscriptAnthology.LAnthologyDraftRead(_cTranscriptDesk.CDeskRead());
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
            CTranscriptDraftChanged?.Invoke(example);
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

        _cTranscriptDesk.CDeskChip?.LQuillMentionAdd(0, 0, text, start, length, stored);
    }

    public void CTranscriptSilenceSet(string text, int start, int length)
    {
        _cTranscriptDesk.CDeskChip?.LQuillMentionAdd(0, 0, text, start, length, 0);
    }

    public void CTranscriptSenseSet(string text, int start, int length, long senseId)
    {
        _cTranscriptDesk.CDeskChip?.LQuillSenseSet(0, 0, text, start, length, senseId);
    }

    public void CTranscriptMentionRemove(long mentionId)
    {
        _cTranscriptDesk.CDeskQuill?.LQuillMentionRemove(0, 0, mentionId);
    }

    public void CTranscriptMentionRemove(string text, int start, int length)
    {
        _cTranscriptDesk.CDeskChip?.LQuillMentionRemove(0, 0, text, start, length);
    }

    public bool CTranscriptMentionCheck(string text, int start, int length)
    {
        return _cTranscriptDesk.CDeskTenure?.LTenureMentionCheck(0, 0, text, start, length) is true;
    }

    public bool CTranscriptSenseCheck(string text, int start, int length)
    {
        return _cTranscriptDesk.CDeskTenure?.LTenureSenseCheck(0, 0, text, start, length) is true;
    }

    public CMentionSense? CTranscriptSenseRead(string text, int start, int length)
    {
        return CMention.LMentionSenseRead(
            _cTranscriptDraftPort,
            _cTranscriptDesk,
            0,
            0,
            text,
            start,
            length,
            _cTranscriptEnvoy,
            _cTranscriptSettingsPort);
    }

    public IReadOnlyList<CMentionLabel> CTranscriptMentionRead()
    {
        return CMention.LMentionChipRead(
            _cTranscriptDraftPort, _cTranscriptDesk, 0, 0, _cTranscriptEnvoy, _cTranscriptSettingsPort);
    }
}
