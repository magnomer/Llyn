using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CLedger
{
    private const int CLedgerWeb = 2;

    private static readonly (string, string[])[] CLedgerPages =
    [
        ("Workspace", ["Workspace.Helper"]),
        ("Language", []),
        ("Transcription", ["Respelling.Switch", "Respelling.Helper"]),
        ("Listing", ["Epithet.Switch", "Epithet.Helper"]),
        ("Web", ["Frequency.Switch", "Frequency.Helper", "Morphology.Switch", "Morphology.Helper"]),
        ("Layout", ["Layout.Linked", "Layout.LinkedHelper"]),
    ];

    private readonly CAtelier _cLedgerAtelier;

    private string? _cLedgerWanted;

    internal CLedger(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cLedgerAtelier = atelier;
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
        CLedgerChanged?.Invoke(LLedgerRead());
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

    public CLedgerNotice CLedgerNoticeRead(Exception exception)
    {
        return LLedgerNoticeRead(_cLedgerAtelier.CAtelierSettingsPort, exception);
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
        _cLedgerWanted = text;
        return LLedgerShownRead();
    }

    public CLedgerPage CLedgerMetaRead(bool linked)
    {
        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        return new CLedgerPage(
            "Layout",
            port.LEngineTextRead("Settings.Layout"),
            port.LEngineTextRead(linked ? "Layout.LinkedMeta" : "Layout.FreeMeta"));
    }

    private CLedgerShown LLedgerShownRead()
    {
        IReadOnlyList<string> children = _cLedgerAtelier.CAtelierSettingsPort.LEngineGroupFind(
            CLedgerPages
                .Select(static page => (page.Item1, (IReadOnlyList<string>)page.Item2
                    .Prepend("Settings." + page.Item1 + "Helper")
                    .Prepend("Settings." + page.Item1)
                    .ToList()))
                .ToList(),
            _cLedgerWanted);
        return new CLedgerShown(children, children.Count == 0);
    }

    private CLedgerState LLedgerRead()
    {
        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        string localization = port.LEngineLocalizationRead();
        IReadOnlyDictionary<string, string> texts = port.LEngineLocalizationLoad(localization);
        CSettings settings = CLedgerSettingsRead();

        return new CLedgerState(
            settings,
            port.LEngineWorkspaceRead(),
            localization,
            texts,
            port.LEngineLocalizationScan()
                .Select(code => new KeyValuePair<string, string>(code, CLedgerLanguageRead(code)))
                .ToList(),
            CLedgerPages
                .Select(page => new CLedgerPage(
                    page.Item1, port.LEngineTextRead("Settings." + page.Item1), CLedgerMetaRead(page.Item1, settings)))
                .ToList(),
            LLedgerShownRead());
    }

    private CSettings CLedgerSettingsRead()
    {
        LSettings settings = _cLedgerAtelier.CAtelierSettingsPort.LEngineSettingsRead();
        return new CSettings(
            settings.LSettingsLocalization,
            settings.LSettingsRespelled,
            settings.LSettingsFrequency,
            settings.LSettingsMorphology,
            settings.LSettingsEpithet,
            settings.LSettingsOnline);
    }

    private string CLedgerMetaRead(string child, CSettings settings)
    {
        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        switch (child)
        {
            case "Workspace":
                return port.LEngineWorkspaceFormat();

            case "Language":
                return CLedgerLanguageRead(settings.CSettingsLocalization);

            case "Transcription":
                return port.LEngineTextRead(settings.CSettingsRespelled ? "Settings.On" : "Settings.Off");

            case "Listing":
                return port.LEngineTextRead(settings.CSettingsEpithet ? "Settings.On" : "Settings.Off");

            case "Web":
                return string.Format(
                    CultureInfo.CurrentCulture,
                    port.LEngineTextRead("Settings.Tally"),
                    settings.CSettingsOnline,
                    CLedgerWeb);

            default:
                return string.Empty;
        }
    }

    private string CLedgerLanguageRead(string localization)
    {
        if (!_cLedgerAtelier.CAtelierSettingsPort.LEngineLocalizationScan()
            .Contains(localization, StringComparer.Ordinal))
        {
            return localization;
        }

        try
        {
            return CultureInfo.GetCultureInfo(localization).NativeName;
        }
        catch (CultureNotFoundException)
        {
            return localization;
        }
    }
}
