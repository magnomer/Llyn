using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEstablishment : UserControl
{
    private const long PEstablishmentMegabyte = 1024L * 1024;

    private PWindow _pEstablishmentHost = null!;

    private Action<LBulletin>? _pEstablishmentObserver;

    public PEstablishment()
    {
        InitializeComponent();
    }

    internal void PEstablishmentAttach(PWindow host)
    {
        _pEstablishmentHost = host;

        _pEstablishmentObserver = PObserver.PObserverCreate(this, PEstablishmentBulletinHandle);
        host.PWindowDeportment.LWindowObserverAttach(_pEstablishmentObserver);

        PEstablishmentUpdate();
    }

    internal void PEstablishmentClose()
    {
        if (_pEstablishmentObserver is not null)
        {
            _pEstablishmentHost.PWindowDeportment.LWindowObserverDetach(_pEstablishmentObserver);
            _pEstablishmentObserver = null;
        }
    }

    private void PEstablishmentBulletinHandle(LBulletin bulletin)
    {
        PEstablishmentUpdate();
    }

    internal void PEstablishmentUpdate()
    {
        LEstablishment establishment;
        try
        {
            establishment = _pEstablishmentHost.PWindowDeportment.LWindowEstablishmentRead();
        }
        catch (Exception)
        {
            return;
        }

        PEstablishmentPending.Visibility = establishment.LEstablishmentPending
            ? Visibility.Visible
            : Visibility.Collapsed;
        PEstablishmentUnsaved.Text = string.Format(
            CultureInfo.CurrentCulture,
            PLocalizationCatalog.PLocalizationTextRead("Establishment.Unsaved"),
            establishment.LEstablishmentUnsaved);

        PEstablishmentEntry.Text = string.Format(
            CultureInfo.CurrentCulture,
            PLocalizationCatalog.PLocalizationTextRead(
                establishment.LEstablishmentSingle ? "Establishment.EntryOne" : "Establishment.Entry"),
            establishment.LEstablishmentEntry);

        PEstablishmentSize.Text = PEstablishmentSizeFormat(establishment.LEstablishmentSize);
    }

    private string PEstablishmentSizeFormat(long bytes)
    {
        if (bytes >= PEstablishmentMegabyte)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                PLocalizationCatalog.PLocalizationTextRead("Establishment.Megabyte"),
                ((double)bytes / PEstablishmentMegabyte).ToString("0.0", CultureInfo.CurrentCulture));
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            PLocalizationCatalog.PLocalizationTextRead("Establishment.Kilobyte"),
            (bytes + 1023) / 1024);
    }
}
