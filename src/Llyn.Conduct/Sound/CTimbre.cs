using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTimbre
{
    private readonly CDesk _cTimbreDesk;

    private readonly LPhonologyPort _cTimbrePhonologyPort;

    private readonly LDisplay _cTimbreDisplay;

    internal CTimbre(CDesk desk, LPhonologyPort phonology, LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(display);

        _cTimbreDesk = desk;
        _cTimbrePhonologyPort = phonology;
        _cTimbreDisplay = display;
    }

    public bool CTimbreTonal => _cTimbrePhonologyPort.LEngineTonalCheck(LTimbreLanguage);

    public bool CTimbreSpoken => !_cTimbrePhonologyPort.LEngineSilentCheck(LTimbreLanguage);

    public bool CTimbrePhonemic =>
        _cTimbrePhonologyPort.LEngineRespellingCheck(LTimbreLanguage)
        && _cTimbrePhonologyPort.LEnginePhonemicCheck(LTimbreLanguage);

    public bool CTimbreFanqieRebuildable =>
        LTimbreEntry is not null && _cTimbrePhonologyPort.LEngineBookCheck(LTimbreLanguage);

    public bool CTimbreScriptRebuildable =>
        LTimbreEntry is not null && _cTimbrePhonologyPort.LEngineStyleCheck(LTimbreLanguage);

    public bool CTimbreFanqiePending => _cTimbreDisplay.LDisplayFanqieCheck(LTimbreEntry);

    public bool CTimbreScriptPending => _cTimbreDisplay.LDisplayScriptCheck(LTimbreEntry);

    public bool CTimbreParadigmPending => _cTimbreDisplay.LDisplayParadigmCheck(LTimbreEntry);

    private string LTimbreLanguage => _cTimbreDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    private long? LTimbreEntry => _cTimbreDesk.CDeskStoredRead();
}
