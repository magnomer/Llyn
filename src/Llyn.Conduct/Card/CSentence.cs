using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CSentence
{
    private readonly CDesk _cSentenceDesk;

    private readonly LPhonologyPort _cSentencePhonologyPort;

    internal CSentence(CDesk desk, LPhonologyPort phonology)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(phonology);

        _cSentenceDesk = desk;
        _cSentencePhonologyPort = phonology;
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

    public CSentenceFrame CSentenceFrameRead()
    {
        string language = _cSentenceDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;
        return new CSentenceFrame(
            CCatalog.CCatalogOrderRead(_cSentencePhonologyPort.LEngineOrderRead(language)),
            CSentenceListRead(() => _cSentencePhonologyPort.LEngineParticleRead(language)),
            CSentenceListRead(() => _cSentencePhonologyPort.LEngineDependenceRead(language)));
    }

    private static IReadOnlyList<string> CSentenceListRead(Func<IReadOnlyList<string>> read)
    {
        try
        {
            return read();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
