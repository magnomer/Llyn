using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEstablishment
{
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
            LObserver.LObserverCreate<CEstablishment>(host.PWindowSurface, QEstablishmentRefine));
    }

    internal void QEstablishmentClose()
    {
        _qEstablishmentRelease();
    }

    private void QEstablishmentRefine(CEstablishment establishment)
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
            QLocalizationCatalog.QLocalizationTextRead(establishment.CEstablishmentEntryKey),
            establishment.CEstablishmentEntry);

        QEstablishmentSize.Text = string.Format(
            CultureInfo.CurrentCulture,
            QLocalizationCatalog.QLocalizationTextRead(establishment.CEstablishmentSizeKey),
            establishment.CEstablishmentAmount);
    }
}
