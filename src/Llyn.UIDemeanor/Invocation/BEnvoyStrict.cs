using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEnvoyStrict : CEnvoy
{
    private readonly BConsole _bEnvoyStrictConsole;
    private readonly BLocalizationCatalog _bEnvoyStrictCatalog;
    private readonly BInvocation _bEnvoyStrictInvocation;
    private readonly Func<string, bool> _bEnvoyStrictPresence;

    internal BEnvoyStrict(
        BConsole console, BLocalizationCatalog catalog, BInvocation invocation, Func<string, bool> presence)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(invocation);
        ArgumentNullException.ThrowIfNull(presence);

        _bEnvoyStrictConsole = console;
        _bEnvoyStrictCatalog = catalog;
        _bEnvoyStrictInvocation = invocation;
        _bEnvoyStrictPresence = presence;
    }

    internal bool BEnvoyStrictRefusal { get; private set; }

    public bool CEnvoyConfirm(string key)
    {
        return BEnvoyStrictConsult(_bEnvoyStrictCatalog.BLocalizationTextRead(key));
    }

    public bool CEnvoyConfirm(string key, string tallyKey, int tally)
    {
        return BEnvoyStrictConsult(string.Join(
            " ",
            _bEnvoyStrictCatalog.BLocalizationTextRead(key),
            _bEnvoyStrictCatalog.BLocalizationTextRead(tallyKey),
            tally.ToString(CultureInfo.CurrentCulture)));
    }

    public bool CEnvoyUnionConfirm(string key, string dropped, string kept)
    {
        return BEnvoyStrictConsult($"{_bEnvoyStrictCatalog.BLocalizationTextRead(key)} {dropped} → {kept}");
    }

    public void CEnvoyFailureShow(string key)
    {
        BEnvoyStrictRefusal = true;
        _bEnvoyStrictConsole.BConsoleNoticePrint(
            BBootstrap.BBootstrapNoticeHead + _bEnvoyStrictCatalog.BLocalizationTextRead(key));
    }

    public void CEnvoyFailureShow(string key, CLedgerNotice notice)
    {
        ArgumentNullException.ThrowIfNull(notice);

        CEnvoyFailureShow(key);
        _bEnvoyStrictConsole.BConsoleNoticePrint(
            BBootstrap.BBootstrapNoticeHead + _bEnvoyStrictCatalog.BLocalizationTextRead(notice.CLedgerNoticeKey));
        if (notice is { CLedgerNoticeLabel: string label, CLedgerNoticePath: string path })
        {
            _bEnvoyStrictConsole.BConsoleNoticePrint(
                $"{BBootstrap.BBootstrapNoticeHead}{_bEnvoyStrictCatalog.BLocalizationTextRead(label)} {path}");
        }
    }

    public bool? CEnvoyLeaveConfirm()
    {
        bool? answer = _bEnvoyStrictInvocation.BInvocationGlobal.BInvocationGlobalAnswer;
        BEnvoyStrictRefusal |= answer is null;
        return answer;
    }

    public string? CEnvoyMarkupRead()
    {
        string? path = _bEnvoyStrictInvocation.BInvocationValueRead("import");
        BEnvoyStrictRefusal |= path is null;
        return path;
    }

    public bool CEnvoyCustomsRead(CSCustoms customs)
    {
        ArgumentNullException.ThrowIfNull(customs);

        BInvocationGlobal global = _bEnvoyStrictInvocation.BInvocationGlobal;
        if (!global.BInvocationGlobalQuiet || global.BInvocationGlobalAnswer is null)
        {
            for (int row = 0; row < customs.CSCustomsEntry.Count; row++)
            {
                CMarkupEntry entry = customs.CSCustomsEntry[row];
                CSCustomsRow proposal = customs.CSCustomsRowRead(row);
                string mode = _bEnvoyStrictCatalog.BLocalizationTextRead(proposal.CSCustomsRowMode switch
                {
                    CSCustomsMode.CSCustomsModeFresh => "Customs.Create",
                    CSCustomsMode.CSCustomsModeMerge => "Customs.Append",
                    _ => "Customs.Replace",
                });
                string target = proposal.CSCustomsRowTargeted
                    ? " " + proposal.CSCustomsRowTarget.ToString(CultureInfo.InvariantCulture)
                    : string.Empty;
                string name = $"{entry.CMarkupEntryName} ({entry.CMarkupEntryLanguage})";
                _bEnvoyStrictConsole.BConsoleNoticePrint($"{BBootstrap.BBootstrapNoticeHead}{name} {mode}{target}");
            }
        }

        return BEnvoyStrictConsult(_bEnvoyStrictCatalog.BLocalizationTextRead("Invocation.CustomsConfirm"));
    }

    public bool CEnvoyPressRead(CSPress press)
    {
        BEnvoyStrictRefusal = true;
        return false;
    }

    public void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(omissions);

        if (_bEnvoyStrictInvocation.BInvocationGlobal.BInvocationGlobalQuiet)
        {
            return;
        }

        foreach (CMarkupOmission omission in omissions)
        {
            _bEnvoyStrictConsole.BConsoleNoticePrint(
                $"{BBootstrap.BBootstrapNoticeHead}{omission.CMarkupOmissionLine}: {omission.CMarkupOmissionText}");
        }
    }

    public (string? CEnvoyFile, CPortraitMedium CEnvoyMedium) CEnvoyFileRead(
        string file, IReadOnlyList<CPortraitChoice> choices)
    {
        ArgumentNullException.ThrowIfNull(choices);

        if (_bEnvoyStrictInvocation.BInvocationValueRead("to") is not string to)
        {
            BEnvoyStrictRefusal = true;
            return (null, default);
        }

        CPortraitMedium medium = _bEnvoyStrictInvocation.BInvocationValueRead("as") switch
        {
            "markup" => CPortraitMedium.CPortraitMediumMarkup,
            "html" => CPortraitMedium.CPortraitMediumHtml,
            "markdown" => CPortraitMedium.CPortraitMediumMarkdown,
            "docx" => CPortraitMedium.CPortraitMediumDocx,
            "pdf" => CPortraitMedium.CPortraitMediumPdf,
            _ => choices.First(static choice => choice.CPortraitChoiceChosen).CPortraitChoiceMedium,
        };
        if (_bEnvoyStrictPresence(to) && !BEnvoyStrictConsult(string.Format(
                CultureInfo.CurrentCulture,
                _bEnvoyStrictCatalog.BLocalizationTextRead("Invocation.ReplaceConfirm"),
                to)))
        {
            return (null, default);
        }

        return (to, medium);
    }

    public string? CEnvoyWorkspaceRead(string workspace)
    {
        string? chosen = _bEnvoyStrictInvocation.BInvocationValueRead("workspace");
        BEnvoyStrictRefusal |= chosen is null;
        return chosen;
    }

    public string? CEnvoyFolderRead(string folder)
    {
        BEnvoyStrictRefusal = true;
        return null;
    }

    public string? CEnvoyCoinageRead(string key)
    {
        BEnvoyStrictRefusal = true;
        return null;
    }

    public string? CEnvoyMediaRead(string key, IReadOnlyList<string> suffixes)
    {
        BEnvoyStrictRefusal = true;
        return null;
    }

    private bool BEnvoyStrictConsult(string question)
    {
        bool answer;
        if (_bEnvoyStrictInvocation.BInvocationGlobal.BInvocationGlobalAnswer is bool given)
        {
            answer = given;
        }
        else if (_bEnvoyStrictConsole.BConsoleTerminalCheck())
        {
            _bEnvoyStrictConsole.BConsoleNoticePrint($"{BBootstrap.BBootstrapNoticeHead}{question} [y/N]");
            string? line = _bEnvoyStrictConsole.BConsoleLineRead()?.Trim();
            answer = string.Equals(line, "y", StringComparison.OrdinalIgnoreCase)
                || string.Equals(line, "yes", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            answer = false;
        }

        BEnvoyStrictRefusal |= !answer;
        return answer;
    }
}
