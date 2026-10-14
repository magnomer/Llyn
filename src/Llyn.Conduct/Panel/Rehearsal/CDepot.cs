using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDepot
{
    private readonly LDeckPort _cDepotPort;

    private readonly LSettingsPort _cDepotSettings;

    private string _cDepotLine = string.Empty;

    public CDepot(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cDepotPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cDepotSettings = atelier.CAtelierSettingsPort;
    }

    public event Action<CDepotState>? CDepotChanged;

    public CDepotState CDepotRead()
    {
        return new CDepotState(_cDepotPort.LEngineDepotCheck(), _cDepotLine);
    }

    public async Task CDepotExport(long deck, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        string line = string.Empty;
        try
        {
            LDeckReceipt receipt;
            try
            {
                Task<LDeckReceipt> export = _cDepotPort.LEngineDeckExport(
                    deck, CPortrait.LPortraitLabelRead(_cDepotSettings), CancellationToken.None);
                LDepotRaise(string.Empty);
                receipt = await export;
            }
            catch (Exception exception)
            {
                CLedger.LLedgerFailureShow(envoy, _cDepotSettings, "Deck.ExportFailed", exception);
                return;
            }

            line = LDepotReceiptFormat(_cDepotSettings, receipt);
        }
        finally
        {
            LDepotRaise(line);
        }
    }

    private static string LDepotReceiptFormat(LSettingsPort settings, LDeckReceipt receipt)
    {
        try
        {
            return LDepotLineFormat(settings, receipt);
        }
        catch (FormatException)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}/{2}/{3}",
                receipt.LDeckReceiptCards,
                receipt.LDeckReceiptCopied,
                receipt.LDeckReceiptRemoved,
                receipt.LDeckReceiptFailed.Count);
        }
    }

    private static string LDepotLineFormat(LSettingsPort settings, LDeckReceipt receipt)
    {
        string line = string.Format(
            CultureInfo.CurrentCulture,
            settings.LEngineTextRead("Deck.ExportReceipt"),
            receipt.LDeckReceiptCards,
            receipt.LDeckReceiptCopied,
            receipt.LDeckReceiptRemoved,
            receipt.LDeckReceiptFailed.Count);
        if (receipt.LDeckReceiptFailed.Count == 0)
        {
            return line;
        }

        string failed = string.Join(", ", receipt.LDeckReceiptFailed.Take(10));
        if (receipt.LDeckReceiptFailed.Count > 10)
        {
            failed += ", …";
        }

        return line + " " + string.Format(
            CultureInfo.CurrentCulture, settings.LEngineTextRead("Deck.Failed"), failed);
    }

    private void LDepotRaise(string line)
    {
        _cDepotLine = line;
        CDepotChanged?.Invoke(CDepotRead());
    }
}
