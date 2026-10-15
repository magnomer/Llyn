using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CLedgerReader
{
    private const int CLedgerWeb = 2;

    private static readonly (string, string[])[] CLedgerPages =
    [
        ("Workspace", ["Workspace.Helper"]),
        ("Language", []),
        ("Transcription", ["Respelling.Switch", "Respelling.Helper"]),
        ("Listing", ["Epithet.Switch", "Epithet.Helper"]),
        ("Inflection", ["Analysis.Switch", "Analysis.Helper"]),
        ("Web",
        [
            "Frequency.Switch", "Frequency.Helper", "Morphology.Switch", "Morphology.Helper"
        ]),
        ("Sync", ["Notebook.Name", "Notebook.Helper", "Settings.Depot", "Settings.DepotHelper"]),
        ("Layout", ["Layout.Linked", "Layout.LinkedHelper"]),
    ];

    private readonly CAtelier _cLedgerAtelier;

    private string? _cLedgerWanted;

    internal CLedgerReader(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cLedgerAtelier = atelier;
    }

    internal CLedgerShown CLedgerReaderFind(string? text)
    {
        _cLedgerWanted = text;
        return LLedgerShownRead();
    }

    internal bool CLedgerAnalysisRead()
    {
        return _cLedgerAtelier.CAtelierSettingsPort.LEngineAnalysisCheck();
    }

    internal CLedgerPage CLedgerMetaRead(bool linked)
    {
        LSettingsPort port = _cLedgerAtelier.CAtelierSettingsPort;
        return new CLedgerPage(
            "Layout",
            port.LEngineTextRead("Settings.Layout"),
            port.LEngineTextRead(linked ? "Layout.LinkedMeta" : "Layout.FreeMeta"));
    }

    internal CLedgerState CLedgerReaderRead()
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

    private CSettings CLedgerSettingsRead()
    {
        LSettings settings = _cLedgerAtelier.CAtelierSettingsPort.LEngineSettingsRead();
        return new CSettings(
            settings.LSettingsLocalization,
            settings.LSettingsRespelled,
            settings.LSettingsFrequency,
            settings.LSettingsMorphology,
            settings.LSettingsEpithet,
            settings.LSettingsOnline,
            settings.LSettingsNotebook,
            settings.LSettingsDepot);
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

            case "Inflection":
                return port.LEngineTextRead(CLedgerAnalysisRead() ? "Settings.On" : "Settings.Off");

            case "Web":
                return string.Format(
                    CultureInfo.CurrentCulture,
                    port.LEngineTextRead("Settings.Tally"),
                    settings.CSettingsOnline,
                    CLedgerWeb);

            case "Sync":
                return settings.CSettingsNotebook;

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
