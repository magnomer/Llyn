using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCourier
{
    private readonly CAtelier _cCourierAtelier;

    private CCourierState _cCourierState = new(false, true, string.Empty);

    internal CCourier(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cCourierAtelier = atelier;
    }

    public event Action<CCourierState>? CCourierChanged;

    public CCourierState CCourierRead()
    {
        return _cCourierState;
    }

    public async Task CCourierSend(CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        if (_cCourierState.CCourierStateBusy)
        {
            return;
        }

        LSettingsPort settings = _cCourierAtelier.CAtelierSettingsPort;
        string line = string.Empty;
        try
        {
            LCourierRaise(new CCourierState(true, false, settings.LEngineTextRead("Courier.Sending")));
            LReceipt receipt;
            try
            {
                receipt = await _cCourierAtelier.CAtelierPortraitPort.LEngineCourierSend(
                    CPortrait.LPortraitLabelRead(settings), CancellationToken.None);
            }
            catch (Exception exception)
            {
                CLedger.LLedgerFailureShow(envoy, settings, "Courier.SendFailed", exception);
                return;
            }

            line = LCourierReceiptFormat(settings, receipt);
        }
        finally
        {
            LCourierRaise(new CCourierState(false, true, line));
        }
    }

    public async Task CCourierAttach(CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        if (_cCourierState.CCourierStateBusy)
        {
            return;
        }

        LSettingsPort settings = _cCourierAtelier.CAtelierSettingsPort;
        string line = string.Empty;
        try
        {
            LCourierRaise(new CCourierState(true, false, settings.LEngineTextRead("Courier.Waiting")));
            await _cCourierAtelier.CAtelierPortraitPort.LEngineCourierAttach(CancellationToken.None);
            line = settings.LEngineTextRead("Courier.Attached");
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Courier.AttachFailed", exception);
        }
        finally
        {
            LCourierRaise(new CCourierState(false, true, line));
        }
    }

    private static string LCourierReceiptFormat(LSettingsPort settings, LReceipt receipt)
    {
        try
        {
            return LCourierLineFormat(settings, receipt);
        }
        catch (FormatException)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Joplin: {0}/{1}/{2}/{3}",
                receipt.LReceiptSaved,
                receipt.LReceiptKept,
                receipt.LReceiptRemoved,
                receipt.LReceiptFailed.Count);
        }
    }

    private static string LCourierLineFormat(LSettingsPort settings, LReceipt receipt)
    {
        string line = string.Format(
            CultureInfo.CurrentCulture,
            settings.LEngineTextRead("Courier.Receipt"),
            receipt.LReceiptSaved,
            receipt.LReceiptKept,
            receipt.LReceiptRemoved,
            receipt.LReceiptFailed.Count);
        if (receipt.LReceiptFailed.Count == 0)
        {
            return line;
        }

        string failed = string.Join(", ", receipt.LReceiptFailed.Take(10));
        if (receipt.LReceiptFailed.Count > 10)
        {
            failed += ", \u2026";
        }

        return line + " " + string.Format(
            CultureInfo.CurrentCulture, settings.LEngineTextRead("Courier.Failed"), failed);
    }

    private void LCourierRaise(CCourierState state)
    {
        _cCourierState = state;
        CCourierChanged?.Invoke(state);
    }
}
