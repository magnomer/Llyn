using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEstablishment
{
    private const long QEstablishmentMegabyte = 1024L * 1024;

    private readonly FrameworkElement _qEstablishmentSurface;

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
        _qEstablishmentRelease = host.PWindowAtelier.CAtelierEstablishmentAttach(
            LObserver.LObserverCreate<CEstablishment>(host.PWindowSurface, QEstablishmentShow));
    }

    internal void QEstablishmentClose()
    {
        _qEstablishmentRelease();
    }

    private void QEstablishmentShow(CEstablishment establishment)
    {
        QEstablishmentPending.Visibility = establishment.CEstablishmentPending
            ? Visibility.Visible
            : Visibility.Collapsed;
        QEstablishmentUnsaved.Text = string.Format(
            CultureInfo.CurrentCulture,
            QLocalizationCatalog.QLocalizationTextRead("Establishment.Unsaved"),
            establishment.CEstablishmentUnsaved);

        QEstablishmentEntry.Text = string.Format(
            CultureInfo.CurrentCulture,
            QLocalizationCatalog.QLocalizationTextRead(
                establishment.CEstablishmentSingle ? "Establishment.EntryOne" : "Establishment.Entry"),
            establishment.CEstablishmentEntry);

        QEstablishmentSize.Text = QEstablishmentSizeFormat(establishment.CEstablishmentSize);
    }

    private static string QEstablishmentSizeFormat(long bytes)
    {
        if (bytes >= QEstablishmentMegabyte)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                QLocalizationCatalog.QLocalizationTextRead("Establishment.Megabyte"),
                ((double)bytes / QEstablishmentMegabyte).ToString("0.0", CultureInfo.CurrentCulture));
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            QLocalizationCatalog.QLocalizationTextRead("Establishment.Kilobyte"),
            (bytes + 1023) / 1024);
    }
}
