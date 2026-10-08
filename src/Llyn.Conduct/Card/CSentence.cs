using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CSentence
{
    private readonly CDesk _cSentenceDesk;

    private readonly LSentencePort _cSentencePort;

    private readonly LDraftPort _cSentenceDraftPort;

    private readonly LSettingsPort _cSentenceSettingsPort;

    private readonly CEnvoy _cSentenceEnvoy;

    private readonly CLedgerNoticed _cSentenceNoticed;

    internal CSentence(
        CDesk desk,
        LSentencePort sentences,
        LDraftPort drafts,
        LSettingsPort settings,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(sentences);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _cSentenceDesk = desk;
        _cSentencePort = sentences;
        _cSentenceDraftPort = drafts;
        _cSentenceSettingsPort = settings;
        _cSentenceEnvoy = envoy;
        _cSentenceNoticed = noticed;
    }

    public event Action? CSentenceReferenceChanged;

    internal void LSentenceObserverAttach(Action<Action> marshal)
    {
        _cSentenceDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectReference, _ => marshal(() => CSentenceReferenceChanged?.Invoke()));
    }

    public void CSentenceAdd(long cardId, int below)
    {
        if (_cSentenceDesk.CDeskTenure?.LTenureRead() is not LDraft held
            || held.LDraftContent.LEntryDraftMeanings
                .Concat(held.LDraftContent.LEntryDraftCollocations)
                .FirstOrDefault(card => card.LCardDraftId == cardId) is not LCardDraft card
            || below < 0
            || below >= card.LCardDraftSentence.Count)
        {
            return;
        }

        _cSentenceDesk.CDeskSentence?.LQuillSentenceAdd(cardId, below + 1);
    }

    public void CSentenceRemove(long cardId, long sentenceId)
    {
        _cSentenceDesk.CDeskSentence?.LQuillSentenceRemove(cardId, sentenceId);
    }

    public void CSentenceTextSet(long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskSentence?.LQuillSentenceSet(cardId, sentenceId, text);
    }

    public void CSentenceParticleSet(long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskSentence?.LSentenceParticleSet(cardId, sentenceId, text);
    }

    public void CSentenceDependenceSet(long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskSentence?.LSentenceDependenceSet(cardId, sentenceId, text);
    }

    public void CSentenceCitationSet(long cardId, long sentenceId, long referenceId)
    {
        _cSentenceDesk.CDeskSentence?.LSentenceCitationSet(cardId, sentenceId, referenceId);
    }

    public void CSentenceGlossSet(long cardId, long sentenceId, long glossId, string text)
    {
        _cSentenceDesk.CDeskSentence?.LSentenceGlossSet(cardId, sentenceId, glossId, null, text);
    }

    public void CSentenceLanguageSet(long cardId, long sentenceId, long glossId, string language)
    {
        _cSentenceDesk.CDeskSentence?.LSentenceGlossSet(cardId, sentenceId, glossId, language, null);
    }

    public void CSentenceGlossAdd(long cardId, long sentenceId)
    {
        _cSentenceDesk.CDeskSentence?.LQuillGlossAdd(cardId, sentenceId);
    }

    public void CSentenceGlossRemove(long cardId, long sentenceId, long glossId)
    {
        _cSentenceDesk.CDeskSentence?.LSentenceGlossRemove(cardId, sentenceId, glossId);
    }

    public void CSentenceMentionAdd(long cardId, long sentenceId, string text, int start, int length, long? entryId)
    {
        if (entryId is not long stored)
        {
            _cSentenceEnvoy.CEnvoyFailureShow("Refusal.TargetMissing");
            return;
        }

        _cSentenceDesk.CDeskChip?.LQuillMentionAdd(cardId, sentenceId, text, start, length, stored);
    }

    public void CSentenceSilenceSet(long cardId, long sentenceId, string text, int start, int length)
    {
        _cSentenceDesk.CDeskChip?.LQuillMentionAdd(cardId, sentenceId, text, start, length, 0);
    }

    public void CSentenceSenseSet(long cardId, long sentenceId, string text, int start, int length, long senseId)
    {
        _cSentenceDesk.CDeskChip?.LQuillSenseSet(cardId, sentenceId, text, start, length, senseId);
    }

    public void CSentenceMentionRemove(long cardId, long sentenceId, long mentionId)
    {
        _cSentenceDesk.CDeskMention?.LQuillMentionRemove(cardId, sentenceId, mentionId);
    }

    public void CSentenceMentionRemove(long cardId, long sentenceId, string text, int start, int length)
    {
        _cSentenceDesk.CDeskChip?.LQuillMentionRemove(cardId, sentenceId, text, start, length);
    }

    public bool CSentenceMentionCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return _cSentenceDesk.CDeskTenure is LTenure held
            && new LQuillMention(held).LQuillMentionCheck(cardId, sentenceId, text, start, length);
    }

    public bool CSentenceSenseCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return _cSentenceDesk.CDeskTenure is LTenure held
            && new LQuillMention(held).LQuillSenseCheck(cardId, sentenceId, text, start, length);
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
        if (_cSentenceDesk.CDeskTenure is not LTenure held || held.LTenureRead() is not LDraft draft)
        {
            return new Dictionary<long, IReadOnlyList<CMentionLabel>>();
        }

        try
        {
            return CMention.LMentionLineRead(draft.LDraftContent, _cSentenceDraftPort.LEngineMentionResolve(held));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cSentenceEnvoy, _cSentenceSettingsPort, "Mention.FindFailed", exception);
            return CMention.LMentionLineRead(
                draft.LDraftContent, new Dictionary<long, IReadOnlyList<LMentionLabel>>());
        }
    }

    public CSentenceFrame CSentenceFrameRead()
    {
        string language = _cSentenceDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;
        return new CSentenceFrame(
            CCatalog.CCatalogOrderRead(_cSentencePort.LEngineOrderRead(language)),
            _cSentenceNoticed.LLedgerRepaintRead(
                _cSentenceEnvoy,
                _cSentenceSettingsPort,
                () => _cSentencePort.LEngineParticleRead(language),
                [],
                "Sentence.ParticleFailed"),
            _cSentenceNoticed.LLedgerRepaintRead(
                _cSentenceEnvoy,
                _cSentenceSettingsPort,
                () => _cSentencePort.LEngineDependenceRead(language),
                [],
                "Sentence.DependenceFailed"));
    }
}
