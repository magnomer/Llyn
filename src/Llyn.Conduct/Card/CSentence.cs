using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
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
        _cSentenceDesk.CDeskVigil.CVigilObserverAttach(
            CSubject.CSubjectReference, _ => marshal(() => CSentenceReferenceChanged?.Invoke()));
    }

    public void CSentenceAdd(bool collocated, long cardId, int below)
    {
        if (_cSentenceDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is not LDraft held
            || (collocated ? held.LDraftContent.LEntryDraftCollocations : held.LDraftContent.LEntryDraftMeanings)
                .FirstOrDefault(card => card.LCardDraftId == cardId) is not LCardDraft card
            || below < 0
            || below >= card.LCardDraftSentence.Count)
        {
            return;
        }

        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LQuillSentenceAdd(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, below + 1);
    }

    public void CSentenceRemove(bool collocated, long cardId, long sentenceId)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LQuillSentenceRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId);
    }

    public void CSentenceTextSet(bool collocated, long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LQuillSentenceSet(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId, text);
    }

    public void CSentenceParticleSet(bool collocated, long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LSentenceParticleSet(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId, text);
    }

    public void CSentenceDependenceSet(bool collocated, long cardId, long sentenceId, string text)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LSentenceDependenceSet(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId, text);
    }

    public void CSentenceCitationSet(bool collocated, long cardId, long sentenceId, long referenceId)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LSentenceCitationSet(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
            cardId,
            sentenceId,
            referenceId);
    }

    public void CSentenceGlossSet(bool collocated, long cardId, long sentenceId, long glossId, string text)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LSentenceGlossSet(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
            cardId,
            sentenceId,
            glossId,
            null,
            text);
    }

    public void CSentenceLanguageSet(bool collocated, long cardId, long sentenceId, long glossId, string language)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LSentenceGlossSet(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
            cardId,
            sentenceId,
            glossId,
            language,
            null);
    }

    public void CSentenceGlossAdd(bool collocated, long cardId, long sentenceId)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LQuillGlossAdd(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId);
    }

    public void CSentenceGlossRemove(bool collocated, long cardId, long sentenceId, long glossId)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftSentence?.LSentenceGlossRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId, glossId);
    }

    public void CSentenceMentionAdd(
        bool collocated, long cardId, long sentenceId, string text, int start, int length, long? entryId)
    {
        if (entryId is not long stored)
        {
            _cSentenceEnvoy.CEnvoyFailureShow("Refusal.TargetMissing");
            return;
        }

        _cSentenceDesk.CDeskDraft.CDeskDraftChip?.LQuillMentionAdd(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
            cardId,
            sentenceId,
            text,
            start,
            length,
            stored);
    }

    public void CSentenceSilenceSet(bool collocated, long cardId, long sentenceId, string text, int start, int length)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftChip?.LQuillMentionAdd(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
            cardId,
            sentenceId,
            text,
            start,
            length,
            0);
    }

    public void CSentenceSenseSet(
        bool collocated, long cardId, long sentenceId, string text, int start, int length, long senseId)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftChip?.LQuillSenseSet(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
            cardId,
            sentenceId,
            text,
            start,
            length,
            senseId);
    }

    public void CSentenceMentionRemove(bool collocated, long cardId, long sentenceId, long mentionId)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftMention?.LQuillMentionRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId, mentionId);
    }

    public void CSentenceMentionRemove(
        bool collocated, long cardId, long sentenceId, string text, int start, int length)
    {
        _cSentenceDesk.CDeskDraft.CDeskDraftChip?.LQuillMentionRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
            cardId,
            sentenceId,
            text,
            start,
            length);
    }

    public bool CSentenceMentionCheck(
        bool collocated, long cardId, long sentenceId, string text, int start, int length)
    {
        return _cSentenceDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            && new LQuillMention(held).LQuillMentionCheck(
                collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
                cardId,
                sentenceId,
                text,
                start,
                length);
    }

    public bool CSentenceSenseCheck(
        bool collocated, long cardId, long sentenceId, string text, int start, int length)
    {
        return _cSentenceDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            && new LQuillMention(held).LQuillSenseCheck(
                collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
                cardId,
                sentenceId,
                text,
                start,
                length);
    }

    public CMentionSense? CSentenceSenseRead(
        bool collocated, long cardId, long sentenceId, string text, int start, int length)
    {
        return CMention.LMentionSenseRead(
            _cSentenceDraftPort,
            _cSentenceDesk,
            new LExampleSiteCard(
                collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId),
            text,
            start,
            length,
            _cSentenceEnvoy,
            _cSentenceSettingsPort);
    }

    public IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> CSentenceMentionRead(CEntryDraft shown)
    {
        ArgumentNullException.ThrowIfNull(shown);

        if (_cSentenceDesk.CDeskDraft.CDeskDraftTenure is not LTenure held || held.LTenureRead() is not LDraft draft)
        {
            return new Dictionary<long, IReadOnlyList<CMentionLabel>>();
        }

        try
        {
            return CMention.LMentionLineRead(
                shown, draft.LDraftContent, _cSentenceDraftPort.LEngineMentionResolve(draft), _cSentenceDraftPort);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cSentenceEnvoy, _cSentenceSettingsPort, "Mention.FindFailed", exception);
            return CMention.LMentionLineRead(
                shown,
                draft.LDraftContent, new Dictionary<long, IReadOnlyList<LMentionLabel>>(), _cSentenceDraftPort);
        }
    }

    public CSentenceFrame CSentenceFrameRead()
    {
        string language = _cSentenceDesk.CDeskDraft.CDeskDraftTenure?.LTenureLanguageRead() ?? string.Empty;
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
