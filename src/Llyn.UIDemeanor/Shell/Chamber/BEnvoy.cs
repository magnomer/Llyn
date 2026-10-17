using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEnvoy : CEnvoy
{
    private readonly BConsole _bEnvoyConsole;
    private readonly BLocalizationCatalog _bEnvoyCatalog;
    private readonly BBlueprintPart _bEnvoyParley;
    private readonly string _bEnvoyArrow;
    private readonly List<string> _bEnvoyNotices = [];
    private BWindow? _bEnvoyWindow;

    internal BEnvoy(BConsole console, BLocalizationCatalog catalog, BBlueprint blueprint)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(blueprint);

        _bEnvoyConsole = console;
        _bEnvoyCatalog = catalog;
        _bEnvoyParley = BContract.BContractBlueprintFind(blueprint, "ASParley");
        _bEnvoyArrow = _bEnvoyParley.BBlueprintPartChildren is [{ BBlueprintPartGlyphs: string set }]
            ? blueprint.BBlueprintGlyphs[set]["Arrow"][0]
            : "→";
    }

    internal void BEnvoyWindowAttach(BWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _bEnvoyWindow = window;
    }

    internal void BEnvoyNoticeAdd(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        lock (_bEnvoyNotices)
        {
            _bEnvoyNotices.Add(text);
        }
    }

    internal void BEnvoyNoticePrint()
    {
        lock (_bEnvoyNotices)
        {
            foreach (string notice in _bEnvoyNotices)
            {
                _bEnvoyConsole.BConsoleNoticePrint(notice);
            }

            _bEnvoyNotices.Clear();
        }
    }

    internal void BEnvoyFaultShow(string? log, string reason, Action<Exception> nested)
    {
        ArgumentNullException.ThrowIfNull(reason);
        ArgumentNullException.ThrowIfNull(nested);

        string fault = _bEnvoyCatalog.BLocalizationTextRead("Workspace.Fault");
        List<BSParleyLine> lines = [new(fault, false)];
        if (!string.IsNullOrEmpty(log))
        {
            lines.AddRange([new(string.Empty, false), new(log, true)]);
        }

        lines.AddRange([new(string.Empty, false), new(reason, false)]);
        if (BEnvoyParleyConsult("alert", lines, false, nested) is not null)
        {
            return;
        }

        BEnvoyNoticeAdd(BBootstrap.BBootstrapNoticeHead + fault);
        BEnvoyNoticeAdd($"{log}");
        BEnvoyNoticeAdd(reason);
    }

    internal void BEnvoyRefusalShow(string key, string workspace, string reason)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(reason);

        List<BSParleyLine> lines = [new(_bEnvoyCatalog.BLocalizationTextRead(key), false)];
        if (workspace.Length > 0)
        {
            lines.AddRange([new(string.Empty, false), new(workspace, true)]);
        }

        lines.AddRange([new(string.Empty, false), new(reason, false)]);
        BEnvoyParleyConsult("alert", lines, false);
    }

    internal void BEnvoyRescueShow(string key, string? backup, string? reason)
    {
        string rescue = _bEnvoyCatalog.BLocalizationTextRead(key);
        List<BSParleyLine> lines = [new(rescue, false)];
        if (!string.IsNullOrEmpty(backup))
        {
            lines.AddRange([new(string.Empty, false), new(backup, true)]);
        }

        if (!string.IsNullOrEmpty(reason))
        {
            lines.AddRange([new(string.Empty, false), new(reason, false)]);
        }

        if (BEnvoyParleyConsult("dialog", lines, false) is not null)
        {
            return;
        }

        BEnvoyNoticeAdd(BBootstrap.BBootstrapNoticeHead + rescue);
        BEnvoyNoticeAdd($"{backup}");
        BEnvoyNoticeAdd($"{reason}");
    }

    internal void BEnvoyThemeShow(string reason)
    {
        ArgumentNullException.ThrowIfNull(reason);

        const string fallback = "The console theme could not be loaded.";
        BSParleyLine[] lines = [new(fallback, false), new(string.Empty, false), new(reason, false)];
        if (BEnvoyParleyConsult("dialog", lines, false) is not null)
        {
            return;
        }

        BEnvoyNoticeAdd($"{BBootstrap.BBootstrapNoticeHead}{fallback} {reason}");
    }

    public bool CEnvoyConfirm(string key)
    {
        return BEnvoyParleyConsult("alert", [new(_bEnvoyCatalog.BLocalizationTextRead(key), false)], true) is 0;
    }

    public bool CEnvoyConfirm(string key, string tallyKey, int tally)
    {
        string count = string.Concat(
            _bEnvoyCatalog.BLocalizationTextRead(tallyKey), " ", tally.ToString(CultureInfo.CurrentCulture));
        BSParleyLine[] lines =
        [
            new(_bEnvoyCatalog.BLocalizationTextRead(key), false),
            new(string.Empty, false),
            new(count, false),
        ];

        return BEnvoyParleyConsult("dialog", lines, true) is 0;
    }

    public bool CEnvoyUnionConfirm(string key, string dropped, string kept)
    {
        BSParleyLine[] lines =
        [
            new(_bEnvoyCatalog.BLocalizationTextRead(key), false),
            new(string.Empty, false),
            new($"{dropped} {_bEnvoyArrow} {kept}", false),
        ];

        return BEnvoyParleyConsult("dialog", lines, true) is 0;
    }

    public void CEnvoyFailureShow(string key)
    {
        string headline = _bEnvoyCatalog.BLocalizationTextRead(key);
        if (BEnvoyParleyConsult("alert", [new(headline, false)], false) is null)
        {
            BEnvoyNoticeAdd(BBootstrap.BBootstrapNoticeHead + headline);
        }
    }

    public void CEnvoyFailureShow(string key, CLedgerNotice notice)
    {
        ArgumentNullException.ThrowIfNull(notice);

        string headline = _bEnvoyCatalog.BLocalizationTextRead(key);
        string detail = _bEnvoyCatalog.BLocalizationTextRead(notice.CLedgerNoticeKey);
        List<BSParleyLine> lines = [new(headline, false), new(string.Empty, false), new(detail, false)];
        List<string> notices = [BBootstrap.BBootstrapNoticeHead + headline, BBootstrap.BBootstrapNoticeHead + detail];
        if (notice is { CLedgerNoticeLabel: string label, CLedgerNoticePath: string path })
        {
            string held = _bEnvoyCatalog.BLocalizationTextRead(label);
            lines.AddRange([new(string.Empty, false), new(held, false), new(path, true)]);
            notices.Add($"{BBootstrap.BBootstrapNoticeHead}{held} {path}");
        }

        if (BEnvoyParleyConsult("alert", lines, false) is not null)
        {
            return;
        }

        notices.ForEach(BEnvoyNoticeAdd);
    }

    public bool? CEnvoyLeaveConfirm()
    {
        BSParleyButton[] buttons =
        [
            new(_bEnvoyCatalog.BLocalizationTextRead("Leave.Save")),
            new(_bEnvoyCatalog.BLocalizationTextRead("Leave.Discard")),
            new(_bEnvoyCatalog.BLocalizationTextRead("Leave.Cancel")),
        ];
        int? answer = BEnvoyParleyConsult(
            "Terms.Product", [new(_bEnvoyCatalog.BLocalizationTextRead("Leave.Message"), false)], buttons, null);

        return answer switch
        {
            0 => true,
            1 => false,
            _ => null,
        };
    }

    public string? CEnvoyMarkupRead()
    {
        return null;
    }

    public bool CEnvoyCustomsRead(CSCustoms customs)
    {
        ArgumentNullException.ThrowIfNull(customs);

        BSParleyButton admit = new(
            _bEnvoyCatalog.BLocalizationTextRead("Customs.Accept"), ConsoleKey.F2, "Legend.Import");
        admit.BSParleyButtonEnabled = customs.CSCustomsReadyCheck();
        BSParleyButton[] buttons = [admit, new(_bEnvoyCatalog.BLocalizationTextRead("Customs.Cancel"))];

        return BEnvoyParleyConsult(
            "Customs.Title", [], buttons, new BSCustoms(customs, _bEnvoyCatalog, admit)) is 0;
    }

    public bool CEnvoyPressRead(CSPress press)
    {
        return false;
    }

    public void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(omissions);

        BSCustomsOmission list = new(omissions, _bEnvoyCatalog.BLocalizationTextRead("Customs.Omission"));
        BSParleyButton[] buttons = [new(_bEnvoyCatalog.BLocalizationTextRead("Customs.Close"))];
        if (BEnvoyParleyConsult("Customs.Title", [], buttons, list) is not null)
        {
            return;
        }

        foreach (CMarkupOmission omission in omissions)
        {
            BEnvoyNoticeAdd(
                $"{BBootstrap.BBootstrapNoticeHead}{omission.CMarkupOmissionLine}: {omission.CMarkupOmissionText}");
        }
    }

    public (string? CEnvoyFile, CPortraitMedium CEnvoyMedium) CEnvoyFileRead(
        string file, IReadOnlyList<CPortraitChoice> choices)
    {
        return (null, default);
    }

    public string? CEnvoyWorkspaceRead(string workspace)
    {
        return null;
    }

    public string? CEnvoyFolderRead(string folder)
    {
        return null;
    }

    public string? CEnvoyCoinageRead(string key)
    {
        BSParleyButton create = new(_bEnvoyCatalog.BLocalizationTextRead("Coinage.Create"));
        create.BSParleyButtonEnabled = CSCoinage.CSCoinageWordingCheck(string.Empty);
        BField field = new(text => create.BSParleyButtonEnabled = CSCoinage.CSCoinageWordingCheck(text));
        BSParleyButton[] buttons = [create, new(_bEnvoyCatalog.BLocalizationTextRead("Coinage.Cancel"))];
        int? answer = BEnvoyParleyConsult(
            "Terms.Product", [new(_bEnvoyCatalog.BLocalizationTextRead(key), false)], buttons, field);

        return answer is 0 ? field.BFieldText : null;
    }

    public string? CEnvoyMediaRead(string key, IReadOnlyList<string> suffixes)
    {
        return null;
    }

    private int? BEnvoyParleyConsult(
        string role, IReadOnlyList<BSParleyLine> lines, bool question, Action<Exception>? fault = null)
    {
        if (_bEnvoyWindow is not BWindow window)
        {
            return null;
        }

        BSParleyButton[] buttons = question
            ? [new(_bEnvoyCatalog.BLocalizationTextRead("Parley.Yes")),
                new(_bEnvoyCatalog.BLocalizationTextRead("Parley.No"))]
            : [new(_bEnvoyCatalog.BLocalizationTextRead("Parley.Ok"))];
        BSParley parley = new(
            _bEnvoyParley,
            _bEnvoyCatalog.BLocalizationTextRead("Terms.Product"),
            role,
            lines,
            buttons,
            buttons.Length - 1);

        return window.BWindowParleyConsult(parley, fault);
    }

    private int? BEnvoyParleyConsult(
        string title,
        IReadOnlyList<BSParleyLine> lines,
        IReadOnlyList<BSParleyButton> buttons,
        BSParleyBody? body)
    {
        if (_bEnvoyWindow is not BWindow window)
        {
            return null;
        }

        string heading = _bEnvoyCatalog.BLocalizationTextRead(title);
        BSParley parley = new(_bEnvoyParley, heading, "dialog", lines, buttons, buttons.Count - 1, body);
        return window.BWindowParleyConsult(parley);
    }
}
