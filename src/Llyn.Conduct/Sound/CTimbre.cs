using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTimbre
{
    private readonly CDesk _cTimbreDesk;

    private readonly LPhonologyPort _cTimbrePhonologyPort;

    private readonly LDisplay _cTimbreDisplay;

    private readonly LMediaPort _cTimbreMediaPort;

    internal CTimbre(CDesk desk, LPhonologyPort phonology, LDisplay display, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(media);

        _cTimbreDesk = desk;
        _cTimbrePhonologyPort = phonology;
        _cTimbreDisplay = display;
        _cTimbreMediaPort = media;
    }

    public event Action? CTimbreParadigmChanged;

    public event Action? CTimbreScriptChanged;

    public event Action? CTimbreReflexChanged;

    public bool CTimbreFlagged => _cTimbreDesk.CDeskTenure?.LTenureFlaggedCheck() ?? false;

    public IReadOnlyList<string> CTimbreVarietyNames => _cTimbreDesk.CDeskTenure?.LTenureVarietyNames ?? [];

    public bool CTimbreReflexShown => _cTimbreDesk.CDeskTenure?.LTenureReflexCheck() ?? false;

    public bool CTimbreTonal => _cTimbrePhonologyPort.LEngineTonalCheck(LTimbreLanguage);

    public bool CTimbreSpoken => !_cTimbrePhonologyPort.LEngineSilentCheck(LTimbreLanguage);

    public bool CTimbrePhonemic =>
        _cTimbrePhonologyPort.LEngineRespellingCheck(LTimbreLanguage)
        && _cTimbrePhonologyPort.LEnginePhonemicCheck(LTimbreLanguage);

    public bool CTimbreReflexPending => _cTimbreDisplay.LDisplaySound.LDisplayReflexCheck(LTimbreEntry);

    public CTimbrePlayback CTimbrePlaybackRead()
    {
        if (_cTimbreDesk.CDeskTenure?.LTenureRead() is not { } held)
        {
            return new CTimbrePlayback(null, false);
        }

        (string? audio, bool audible) = _cTimbreMediaPort.LEngineAudioRead(held.LDraftContent);
        return new CTimbrePlayback(audio, audible);
    }

    public Uri? CTimbrePlaybackStart(string? audio)
    {
        return _cTimbreMediaPort.LEngineAudioResolve(audio);
    }

    public void CTimbreReflexStart()
    {
        _cTimbreDisplay.LDisplaySound.LDisplayReflexStart(LTimbreEntry);
    }

    public void CTimbreReflexRebuild()
    {
        _cTimbreDisplay.LDisplaySound.LDisplayReflexRebuild(LTimbreEntry);
    }

    internal void LTimbreObserverAttach(Action<Action> marshal)
    {
        _cTimbreDesk.CDeskVigil.LVigilEntryAttach(
            CSubject.CSubjectInflection, _ => marshal(() => CTimbreParadigmChanged?.Invoke()));
        _cTimbreDesk.CDeskVigil.LVigilEntryAttach(
            CSubject.CSubjectReflex, _ => marshal(() => CTimbreReflexChanged?.Invoke()));
        _cTimbreDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectScript, _ => marshal(() => CTimbreScriptChanged?.Invoke()));
    }

    private string LTimbreLanguage => _cTimbreDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    private long? LTimbreEntry => _cTimbreDesk.CDeskStoredRead();
}
