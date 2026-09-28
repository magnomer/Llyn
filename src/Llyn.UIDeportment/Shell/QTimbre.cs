using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class QTimbre
{
    private readonly LEditor _qTimbreEditor;

    private readonly LPhonologyPort _qTimbrePort;

    private readonly LDisplay _qTimbreDisplay;

    private readonly CSounding _qTimbreSounding;

    internal QTimbre(LEditor editor, LPhonologyPort phonology, LDisplay display, CSounding sounding)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(sounding);

        _qTimbreEditor = editor;
        _qTimbrePort = phonology;
        _qTimbreDisplay = display;
        _qTimbreSounding = sounding;
    }

    public bool QTimbreTonal => _qTimbrePort.LEngineTonalCheck(QTimbreLanguage);

    public bool QTimbreSilent => _qTimbrePort.LEngineSilentCheck(QTimbreLanguage);

    public bool QTimbreSpoken => !QTimbreSilent;

    public bool QTimbreRespelled => _qTimbrePort.LEngineRespellingCheck(QTimbreLanguage);

    public bool QTimbrePhonemic => QTimbreRespelled && _qTimbrePort.LEnginePhonemicCheck(QTimbreLanguage);

    public bool QTimbreFanqieRebuildable =>
        QTimbreEntry is not null && _qTimbrePort.LEngineBookCheck(QTimbreLanguage);

    public bool QTimbreScriptRebuildable =>
        QTimbreEntry is not null && _qTimbrePort.LEngineStyleCheck(QTimbreLanguage);

    public bool QTimbreFanqiePending => _qTimbreDisplay.LDisplayFanqieCheck(QTimbreEntry);

    public bool QTimbreScriptPending => _qTimbreDisplay.LDisplayScriptCheck(QTimbreEntry);

    public bool QTimbreParadigmPending => _qTimbreDisplay.LDisplayParadigmCheck(QTimbreEntry);

    public string QTimbreParadigmLanguage => _qTimbreSounding.CSoundingLanguageRead();

    private string QTimbreLanguage => _qTimbreEditor.LEditorLanguage;

    private long? QTimbreEntry => _qTimbreEditor.LEditorStudio.CEditorDesk.CDeskStoredRead();

    public IReadOnlyList<CFanqieGroup> QTimbreFanqieRead()
    {
        return _qTimbreSounding.CSoundingFanqieRead();
    }

    public string QTimbreReadingRead(string headword)
    {
        return _qTimbreSounding.CSoundingReadingRead(headword);
    }

    public bool QTimbreAnchorCheck(string headword)
    {
        return _qTimbreSounding.CSoundingAnchorCheck(headword);
    }

    public string QTimbreAnchorFormat(IReadOnlyList<long> anchors, string headword, string separator)
    {
        return _qTimbreSounding.CSoundingAnchorFormat(anchors, headword, separator);
    }

    public IReadOnlyList<CAnchorRow> QTimbreAnchorScan(IReadOnlyList<long> anchors, string reflex, string tone)
    {
        return _qTimbreSounding.CSoundingAnchorScan(anchors, reflex, tone);
    }

    public void QTimbreFanqieRebuild()
    {
        _qTimbreSounding.CSoundingFanqieResolve();
    }

    public void QTimbreFanqieSet(long fanqieId, int rank)
    {
        _qTimbreSounding.CSoundingFanqieSet(fanqieId, rank);
    }

    public IReadOnlyList<CScriptGroup> QTimbreScriptRead()
    {
        return _qTimbreSounding.CSoundingScriptRead();
    }

    public void QTimbreScriptRebuild()
    {
        _qTimbreSounding.CSoundingScriptResolve();
    }

    public IReadOnlyList<CParadigmSlot> QTimbreParadigmRead()
    {
        return _qTimbreSounding.CSoundingParadigmRead();
    }
}
