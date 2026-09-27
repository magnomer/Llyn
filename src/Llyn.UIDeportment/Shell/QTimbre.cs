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

    private readonly LSounding _qTimbreSounding;

    internal QTimbre(LEditor editor, LPhonologyPort phonology, LDisplay display, LSounding sounding)
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

    public string QTimbreParadigmLanguage => _qTimbreSounding.LSoundingLanguageRead(QTimbreEntry);

    private string QTimbreLanguage => _qTimbreEditor.LEditorLanguage;

    private long? QTimbreEntry => _qTimbreEditor.LEditorDesk.LDeskStoredRead();

    public IReadOnlyList<CFanqieGroup> QTimbreFanqieRead()
    {
        return _qTimbreSounding.LSoundingFanqieRead(QTimbreEntry);
    }

    public string QTimbreReadingRead(string headword)
    {
        return _qTimbreSounding.LSoundingReadingRead(QTimbreEntry, headword);
    }

    public bool QTimbreAnchorCheck(string headword)
    {
        return _qTimbreSounding.LSoundingAnchorCheck(_qTimbreSounding.LSoundingAnchorRead(QTimbreEntry), headword);
    }

    public string QTimbreAnchorFormat(IReadOnlyList<long> anchors, string headword, string separator)
    {
        return _qTimbreSounding.LSoundingAnchorFormat(
            _qTimbreSounding.LSoundingAnchorRead(QTimbreEntry), anchors, headword, separator);
    }

    public IReadOnlyList<CAnchorRow> QTimbreAnchorScan(IReadOnlyList<long> anchors, string reflex, string tone)
    {
        return _qTimbreSounding.LSoundingAnchorScan(
            _qTimbreSounding.LSoundingAnchorRead(QTimbreEntry), anchors, QTimbreLanguage, reflex, tone);
    }

    public void QTimbreFanqieRebuild()
    {
        _qTimbreSounding.LSoundingFanqieRebuild(QTimbreEntry);
    }

    public void QTimbreFanqieSet(long fanqieId, int rank)
    {
        _qTimbreSounding.LSoundingFanqieSet(QTimbreEntry, fanqieId, rank);
    }

    public IReadOnlyList<CScriptGroup> QTimbreScriptRead()
    {
        return _qTimbreSounding.LSoundingScriptRead(QTimbreEntry);
    }

    public void QTimbreScriptRebuild()
    {
        _qTimbreSounding.LSoundingScriptRebuild(QTimbreEntry);
    }

    public IReadOnlyList<CParadigmSlot> QTimbreParadigmRead()
    {
        return _qTimbreSounding.LSoundingParadigmRead(QTimbreEntry);
    }
}
