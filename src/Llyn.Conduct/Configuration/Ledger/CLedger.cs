using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CLedger
{
    private readonly CAtelier _cLedgerAtelier;

    private readonly CLedgerReader _cLedgerReader;

    internal CLedger(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cLedgerAtelier = atelier;
        _cLedgerReader = new CLedgerReader(atelier);
    }

    public event Action<CLedgerState>? CLedgerChanged;

    internal CLedgerNoticed LLedgerRepaint { get; } = new();

    internal void LLedgerAttach()
    {
        _cLedgerAtelier.LAtelierObserverAttach(CSubject.CSubjectSettings, _ => LLedgerRepaint.LLedgerNoticedClear());
        _cLedgerAtelier.LAtelierObserverAttach(CSubject.CSubjectWorkspace, _ => LLedgerRepaint.LLedgerNoticedClear());
        _cLedgerAtelier.LAtelierObserverAttach(CSubject.CSubjectVista, _ => LLedgerRepaint.LLedgerNoticedClear());
        _cLedgerAtelier.LAtelierObserverAttach(CSubject.CSubjectSettings, _ => LLedgerRaise());
        _cLedgerAtelier.LAtelierObserverAttach(CSubject.CSubjectWorkspace, _ => LLedgerRaise());
    }

    internal void LLedgerRaise()
    {
        CLedgerChanged?.Invoke(_cLedgerReader.CLedgerReaderRead());
    }

    public void CLedgerLocalizationSave(string language, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            port.LEngineLocalizationSave(language);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
        }
    }

    public void CLedgerEpithetSave(bool epithet, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            port.LEngineEpithetSave(epithet);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
        }
    }

    public void CLedgerFrequencySave(bool frequency, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            port.LEngineFrequencySave(frequency);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
        }
    }


    public void CLedgerMorphologySave(bool morphology, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            port.LEngineMorphologySave(morphology);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
        }
    }

    public void CLedgerAnalysisSave(bool analysis, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            port.LEngineAnalysisSave(analysis);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
        }
    }

    public bool CLedgerAnalysisRead()
    {
        return _cLedgerReader.CLedgerAnalysisRead();
    }

    public void CLedgerRespellingSave(bool respelled, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            port.LEngineRespellingSave(respelled);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
        }
    }

    public string? CLedgerNotebookSave(string notebook, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            return port.LEngineNotebookSave(notebook);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
            return null;
        }
    }

    public string? CLedgerDepotSave(string folder, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            return port.LEngineDepotSave(folder);
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.SaveFailed", exception);
            LLedgerRaise();
            return null;
        }
    }

    public string? CLedgerDepotChange(CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        string? chosen;
        try
        {
            chosen = envoy.CEnvoyFolderRead(_cLedgerAtelier.CAtelierPathRead());
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, _cLedgerAtelier.CAtelierSettingsPort, "Settings.SaveFailed", exception);
            return null;
        }

        return chosen is null ? null : CLedgerDepotSave(chosen, envoy);
    }

    public void CLedgerFailureShow(CEnvoy envoy, string key, Exception exception)
    {
        LLedgerFailureShow(envoy, _cLedgerAtelier.CAtelierSettingsPort, key, exception);
    }

    internal static CLedgerNotice LLedgerNoticeRead(LSettingsPort settings, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(exception);

        (string notice, string? label, string? path) = settings.LEngineFailureRead(
            exception, "Notice.Unexpected", "Notice.Recorded");
        return new CLedgerNotice(notice, label, path);
    }

    internal static void LLedgerFailureShow(CEnvoy envoy, LSettingsPort settings, string key, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);

        envoy.CEnvoyFailureShow(key, LLedgerNoticeRead(settings, exception));
    }

    public void CLedgerFolderOpen(CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        try
        {
            port.LEngineFolderOpen();
        }
        catch (Exception exception)
        {
            LLedgerFailureShow(envoy, port, "Settings.FolderFailed", exception);
        }
    }

    public CLedgerShown CLedgerFind(string? text)
    {
        return _cLedgerReader.CLedgerReaderFind(text);
    }

    public CLedgerPage CLedgerMetaRead(bool linked)
    {
        return _cLedgerReader.CLedgerMetaRead(linked);
    }
}
