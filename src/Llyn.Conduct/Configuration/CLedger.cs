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

    internal CLedger(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cLedgerAtelier = atelier;
    }

    public Action CLedgerAttach(Action<CLedgerState> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        Action settings = _cLedgerAtelier.CAtelierObserverAttach(CSubject.CSubjectSettings, _ => show(CLedgerRead()));
        Action workspace = _cLedgerAtelier.CAtelierObserverAttach(CSubject.CSubjectWorkspace, _ => show(CLedgerRead()));
        show(CLedgerRead());
        return () =>
        {
            settings();
            workspace();
        };
    }

    public void CLedgerLocalizationSave(string language)
    {
        _cLedgerAtelier.CAtelierSettingsPort.LEngineLocalizationSave(language);
    }

    public void CLedgerEpithetSave(bool epithet)
    {
        _cLedgerAtelier.CAtelierSettingsPort.LEngineEpithetSave(epithet);
    }

    public void CLedgerFrequencySave(bool frequency)
    {
        _cLedgerAtelier.CAtelierSettingsPort.LEngineFrequencySave(frequency);
    }

    public void CLedgerMorphologySave(bool morphology)
    {
        _cLedgerAtelier.CAtelierSettingsPort.LEngineMorphologySave(morphology);
    }

    public void CLedgerRespellingSave(bool respelled)
    {
        _cLedgerAtelier.CAtelierSettingsPort.LEngineRespellingSave(respelled);
    }

    public string? CLedgerNoticeRead(Exception exception)
    {
        return _cLedgerAtelier.CAtelierSettingsPort.LEngineNoticeRead(exception);
    }

    public string? CLedgerAuditRecord(Exception exception)
    {
        return _cLedgerAtelier.CAtelierSettingsPort.LEngineAuditRecord(exception);
    }

    public IReadOnlyList<string> CLedgerFind(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        TextInfo casing = CultureInfo.CurrentCulture.TextInfo;
        string wanted = casing.ToLower(text.Trim());

        return CLedgerPages
            .Where(page => wanted.Length == 0 || page.Item2
                .Prepend("Settings." + page.Item1 + "Helper")
                .Prepend("Settings." + page.Item1)
                .Select(key => casing.ToLower(_cLedgerAtelier.CAtelierSettingsPort.LEngineTextRead(key)))
                .Any(label => label.Contains(wanted, StringComparison.Ordinal)))
            .Select(static page => page.Item1)
            .ToList();
    }

    public string CLedgerMetaRead(string child, bool linked)
    {
        ArgumentNullException.ThrowIfNull(child);

        return string.Equals(child, "Layout", StringComparison.Ordinal)
            ? _cLedgerAtelier.CAtelierSettingsPort.LEngineTextRead(linked ? "Layout.LinkedMeta" : "Layout.FreeMeta")
            : CLedgerMetaRead(child, CLedgerSettingsRead());
    }

    private CLedgerState CLedgerRead()
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
                .ToList());
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
        return _cLedgerAtelier.CAtelierSettingsPort.LEngineLocalizationScan()
            .Contains(localization, StringComparer.Ordinal)
                ? CultureInfo.GetCultureInfo(localization).NativeName
                : localization;
    }
}
