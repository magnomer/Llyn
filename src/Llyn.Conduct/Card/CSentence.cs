using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CSentence
{
    private readonly CDesk _cSentenceDesk;

    private readonly LPhonologyPort _cSentencePhonologyPort;

    private readonly LDraftPort _cSentenceDraftPort;

    private readonly LSettingsPort _cSentenceSettingsPort;

    private readonly CEnvoy _cSentenceEnvoy;

    private readonly CLedgerNoticed _cSentenceNoticed;

    internal CSentence(
        CDesk desk,
        LPhonologyPort phonology,
        LDraftPort drafts,
        LSettingsPort settings,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _cSentenceDesk = desk;
        _cSentencePhonologyPort = phonology;
        _cSentenceDraftPort = drafts;
        _cSentenceSettingsPort = settings;
        _cSentenceEnvoy = envoy;
        _cSentenceNoticed = noticed;
    }

    private LTenure? CSentenceTenure => _cSentenceDesk.CDeskFilling ? null : _cSentenceDesk.CDeskTenure;

    public event Action? CSentenceReferenceChanged;

    internal void LSentenceObserverAttach(Action<Action> marshal)
    {
        _cSentenceDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectReference, _ => marshal(() => CSentenceReferenceChanged?.Invoke()));
    }

    public void CSentenceAdd(long cardId, int below)
    {
        _cSentenceDesk.CDeskQuill?.LQuillSentenceAdd(cardId, below + 1);
    }

    public void CSentenceRemove(long cardId, long sentenceId)
    {
        _cSentenceDesk.CDeskQuill?.LQuillSentenceRemove(cardId, sentenceId);
    }

    public void CSentenceTextSet(long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskQuill?.LQuillSentenceSet(cardId, sentenceId, text);
    }

    public void CSentenceParticleSet(long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskQuill?.LQuillParticleSet(cardId, sentenceId, text);
    }

    public void CSentenceDependenceSet(long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskQuill?.LQuillDependenceSet(cardId, sentenceId, text);
    }

    public void CSentenceCitationSet(long cardId, long sentenceId, long referenceId)
    {
        _cSentenceDesk.CDeskQuill?.LQuillCitationSet(cardId, sentenceId, referenceId);
    }

    public void CSentenceGlossSet(long cardId, long sentenceId, long glossId, string text)
    {
        _cSentenceDesk.CDeskQuill?.LQuillGlossSet(cardId, sentenceId, glossId, null, text);
    }

    public void CSentenceLanguageSet(long cardId, long sentenceId, long glossId, string language)
    {
        _cSentenceDesk.CDeskQuill?.LQuillGlossSet(cardId, sentenceId, glossId, language, null);
    }

    public void CSentenceGlossAdd(long cardId, long sentenceId)
    {
        CSentenceTenure?.LTenureGlossAdd(cardId, sentenceId);
    }

    public void CSentenceGlossRemove(long cardId, long sentenceId, long glossId)
    {
        _cSentenceDesk.CDeskQuill?.LQuillGlossRemove(cardId, sentenceId, glossId);
    }

    public void CSentenceMentionAdd(long cardId, long sentenceId, string text, int start, int length, long entryId)
    {
        _cSentenceDesk.CDeskChip?.LQuillMentionAdd(cardId, sentenceId, text, start, length, entryId);
    }

    public void CSentenceSenseSet(long cardId, long sentenceId, string text, int start, int length, long senseId)
    {
        _cSentenceDesk.CDeskChip?.LQuillSenseSet(cardId, sentenceId, text, start, length, senseId);
    }

    public void CSentenceMentionRemove(long cardId, long sentenceId, long mentionId)
    {
        _cSentenceDesk.CDeskQuill?.LQuillMentionRemove(cardId, sentenceId, mentionId);
    }

    public void CSentenceMentionRemove(long cardId, long sentenceId, string text, int start, int length)
    {
        _cSentenceDesk.CDeskChip?.LQuillMentionRemove(cardId, sentenceId, text, start, length);
    }

    public bool CSentenceMentionCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return _cSentenceDesk.CDeskTenure?.LTenureMentionCheck(cardId, sentenceId, text, start, length) is true;
    }

    public bool CSentenceSenseCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return _cSentenceDesk.CDeskTenure?.LTenureSenseCheck(cardId, sentenceId, text, start, length) is true;
    }

    public CMentionSense? CSentenceSenseRead(
        long cardId, long sentenceId, string text, int start, int length)
    {
        return CMention.LMentionSenseRead(
            _cSentenceDraftPort,
            _cSentenceDesk,
            cardId,
            sentenceId,
            text,
            start,
            length,
            _cSentenceEnvoy,
            _cSentenceSettingsPort);
    }

    public IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> CSentenceMentionRead()
    {
        if (_cSentenceDesk.CDeskTenure is not LTenure held)
        {
            return new Dictionary<long, IReadOnlyList<CMentionLabel>>();
        }

        try
        {
            return CMention.LMentionLineRead(_cSentenceDraftPort.LEngineMentionResolve(held));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cSentenceEnvoy, _cSentenceSettingsPort, "Mention.FindFailed", exception);
            return new Dictionary<long, IReadOnlyList<CMentionLabel>>();
        }
    }

    public CSentenceFrame CSentenceFrameRead()
    {
        string language = _cSentenceDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;
        return new CSentenceFrame(
            CCatalog.CCatalogOrderRead(_cSentencePhonologyPort.LEngineOrderRead(language)),
            _cSentenceNoticed.LLedgerRepaintRead(
                _cSentenceEnvoy,
                _cSentenceSettingsPort,
                () => _cSentencePhonologyPort.LEngineParticleRead(language),
                [],
                "Sentence.ParticleFailed"),
            _cSentenceNoticed.LLedgerRepaintRead(
                _cSentenceEnvoy,
                _cSentenceSettingsPort,
                () => _cSentencePhonologyPort.LEngineDependenceRead(language),
                [],
                "Sentence.DependenceFailed"));
    }
}
