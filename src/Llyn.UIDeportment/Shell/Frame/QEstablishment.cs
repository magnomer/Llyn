using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class QEstablishment
{
    private const long QEstablishmentMegabyte = 1024L * 1024;

    private readonly FrameworkElement _qEstablishmentSurface;

    private PWindow _qEstablishmentHost = null!;

    private Action _qEstablishmentRelease = () => { };

    internal QEstablishment(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qEstablishmentSurface = surface;
    }

    private StackPanel QEstablishmentPending =>
        QContract.QContractFind<StackPanel>(_qEstablishmentSurface, "PEstablishmentPending");

    private TextBlock QEstablishmentUnsaved =>
        QContract.QContractFind<TextBlock>(_qEstablishmentSurface, "PEstablishmentUnsaved");

    private TextBlock QEstablishmentEntry =>
        QContract.QContractFind<TextBlock>(_qEstablishmentSurface, "PEstablishmentEntry");

    private TextBlock QEstablishmentSize =>
        QContract.QContractFind<TextBlock>(_qEstablishmentSurface, "PEstablishmentSize");

    internal void QEstablishmentAttach(PWindow host)
    {
        _qEstablishmentHost = host;
        QEstablishmentHook(host, LObserver.LObserverCreate(host.PWindowSurface, QEstablishmentUpdate));
        QEstablishmentUpdate();
    }

    internal void QEstablishmentClose()
    {
        _qEstablishmentRelease();
    }

    private void QEstablishmentHook(PWindow host, Action<LBulletin> observer)
    {
        host.PWindowDeportment.LWindowObserverAttach(observer);
        _qEstablishmentRelease = () => host.PWindowDeportment.LWindowObserverDetach(observer);
    }

    private void QEstablishmentUpdate()
    {
        LEstablishment establishment;
        try
        {
            establishment = _qEstablishmentHost.PWindowDeportment.LWindowEstablishmentRead();
        }
        catch (Exception)
        {
            return;
        }

        QEstablishmentPending.Visibility = establishment.LEstablishmentPending
            ? Visibility.Visible
            : Visibility.Collapsed;
        QEstablishmentUnsaved.Text = string.Format(
            CultureInfo.CurrentCulture,
            PLocalizationCatalog.PLocalizationTextRead("Establishment.Unsaved"),
            establishment.LEstablishmentUnsaved);

        QEstablishmentEntry.Text = string.Format(
            CultureInfo.CurrentCulture,
            PLocalizationCatalog.PLocalizationTextRead(
                establishment.LEstablishmentSingle ? "Establishment.EntryOne" : "Establishment.Entry"),
            establishment.LEstablishmentEntry);

        QEstablishmentSize.Text = QEstablishmentSizeFormat(establishment.LEstablishmentSize);
    }

    private static string QEstablishmentSizeFormat(long bytes)
    {
        if (bytes >= QEstablishmentMegabyte)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                PLocalizationCatalog.PLocalizationTextRead("Establishment.Megabyte"),
                ((double)bytes / QEstablishmentMegabyte).ToString("0.0", CultureInfo.CurrentCulture));
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            PLocalizationCatalog.PLocalizationTextRead("Establishment.Kilobyte"),
            (bytes + 1023) / 1024);
    }
}
