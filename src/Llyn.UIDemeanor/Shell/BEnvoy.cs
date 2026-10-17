using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEnvoy : CEnvoy
{
    private readonly BConsole _bEnvoyConsole;
    private readonly BLocalizationCatalog _bEnvoyCatalog;
    private readonly List<string> _bEnvoyNotices = [];

    internal BEnvoy(BConsole console, BLocalizationCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(catalog);

        _bEnvoyConsole = console;
        _bEnvoyCatalog = catalog;
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

    public bool CEnvoyConfirm(string key)
    {
        return false;
    }

    public bool CEnvoyConfirm(string key, string tallyKey, int tally)
    {
        return false;
    }

    public bool CEnvoyUnionConfirm(string key, string dropped, string kept)
    {
        return false;
    }

    public void CEnvoyFailureShow(string key)
    {
        BEnvoyNoticeAdd(BBootstrap.BBootstrapNoticeHead + _bEnvoyCatalog.BLocalizationTextRead(key));
    }

    public void CEnvoyFailureShow(string key, CLedgerNotice notice)
    {
        ArgumentNullException.ThrowIfNull(notice);

        CEnvoyFailureShow(key);
        CEnvoyFailureShow(notice.CLedgerNoticeKey);
        if (notice is { CLedgerNoticeLabel: string label, CLedgerNoticePath: string path })
        {
            BEnvoyNoticeAdd($"{BBootstrap.BBootstrapNoticeHead}{_bEnvoyCatalog.BLocalizationTextRead(label)} {path}");
        }
    }

    public bool? CEnvoyLeaveConfirm()
    {
        return null;
    }

    public string? CEnvoyMarkupRead()
    {
        return null;
    }

    public bool CEnvoyCustomsRead(CSCustoms customs)
    {
        return false;
    }

    public bool CEnvoyPressRead(CSPress press)
    {
        return false;
    }

    public void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(omissions);

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
        return null;
    }

    public string? CEnvoyMediaRead(string key, IReadOnlyList<string> suffixes)
    {
        return null;
    }
}
