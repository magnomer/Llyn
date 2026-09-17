using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PEstablishment : UserControl
{
    private const long PEstablishmentMegabyte = 1024L * 1024;

    private PWindow _pEstablishmentHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pEstablishmentObserver;

    public PEstablishment()
    {
        InitializeComponent();
    }

    internal void PEstablishmentAttach(PWindow host, LEngine engine)
    {
        _pEstablishmentHost = host;
        _lEngine = engine;

        _pEstablishmentObserver = new PObserver(this, PEstablishmentBulletinHandle);
        engine.LEngineObserverAttach(_pEstablishmentObserver);

        PEstablishmentUpdate();
    }

    internal void PEstablishmentClose()
    {
        if (_pEstablishmentObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pEstablishmentObserver);
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
            establishment = _lEngine.LEngineEstablishmentRead();
        }
        catch (Exception)
        {
            return;
        }

        PEstablishmentPending.Visibility = establishment.LEstablishmentUnsaved > 0
            ? Visibility.Visible
            : Visibility.Collapsed;
        PEstablishmentUnsaved.Text = string.Format(
            CultureInfo.CurrentCulture,
            _pEstablishmentHost.PLocalizationTextRead("Establishment.Unsaved"),
            establishment.LEstablishmentUnsaved);

        PEstablishmentEntry.Text = string.Format(
            CultureInfo.CurrentCulture,
            _pEstablishmentHost.PLocalizationTextRead(
                establishment.LEstablishmentEntry == 1 ? "Establishment.EntryOne" : "Establishment.Entry"),
            establishment.LEstablishmentEntry);

        PEstablishmentSize.Text = PEstablishmentSizeFormat(establishment.LEstablishmentSize);
    }

    private string PEstablishmentSizeFormat(long bytes)
    {
        if (bytes >= PEstablishmentMegabyte)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                _pEstablishmentHost.PLocalizationTextRead("Establishment.Megabyte"),
                ((double)bytes / PEstablishmentMegabyte).ToString("0.0", CultureInfo.CurrentCulture));
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            _pEstablishmentHost.PLocalizationTextRead("Establishment.Kilobyte"),
            (bytes + 1023) / 1024);
    }
}
