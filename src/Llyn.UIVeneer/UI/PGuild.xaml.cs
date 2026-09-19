using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PGuild : UserControl
{
    private PWindow _pGuildHost = null!;

    private LEngine _lEngine = null!;

    public PGuild()
    {
        InitializeComponent();
    }

    internal void PGuildAttach(PWindow host, LEngine engine)
    {
        _pGuildHost = host;
        _lEngine = engine;

        PRoll.ItemsSource = _pRollList;
        POeuvre.ItemsSource = _pOeuvreList;
        PFellow.ItemsSource = _pFellowList;
        PVitaCitation.ItemsSource = _pVitaCitation;
        PAutographUnionList.ItemsSource = _pAutographUnion;

        PColophon.PColophonAttach(host);
    }

    internal void PGuildReset()
    {
        PGuildClear();
        PRollFind();
    }

    internal bool PGuildChangeCheck()
    {
        return PAutograph.Visibility == Visibility.Visible && PAutographChangeCheck();
    }

    internal bool PGuildDraftFinish(bool store)
    {
        if (PAutograph.Visibility != Visibility.Visible)
        {
            return true;
        }

        if (store)
        {
            return PAutographStoreRun();
        }

        PAutographCancel();
        return true;
    }

    internal void PGuildClose()
    {
        PEchelonDropdown.IsOpen = false;
        PLouverDropdown.IsOpen = false;
    }

    internal bool PGuildLeaveConfirm()
    {
        return _pGuildHost.PWindowDiscardConfirm(PGuildChangeCheck(), PGuildDraftFinish);
    }

    private void PGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pOeuvreVista?.LVistaChosen is not null && PColophon.Visibility == Visibility.Visible;
    }

    private async void PGuildPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pOeuvreVista?.LVistaChosen is not null)
        {
            if (PColophon.Visibility == Visibility.Visible)
            {
                await _pGuildHost.PWindowPressRun(ticket => _lEngine.LEnginePortraitPrint(
                    _pOeuvreVista, _pGuildHost.PWindowLegendRead("Source"), ticket));
            }
        }
    }

    private void PGuildScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PGuildScribe);
        if (editing == (PAutograph.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PGuildLeaveConfirm())
            {
                PGuildScribeShow(true);
                return;
            }

            PAutographCancel();
            PGuildScribeShow(false);

            if (_pGuildVista?.LVistaStored is long kept)
            {
                PVitaShow(kept);
                return;
            }

            PGuildClear();
            return;
        }

        if (_pGuildVista?.LVistaStored is not long author)
        {
            PGuildClear();
            return;
        }

        PGuildScribeShow(true);
        PAutographOpen(author);
    }

    internal void PGuildScribeShow(bool editing)
    {
        _pGuildVista?.LVistaEditingSet(editing);

        PAutograph.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PVita.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PGuildViewer.IsChecked = !editing;
        PGuildScribe.IsChecked = editing;
        PAutographChangeUpdate();
    }

    internal void PGuildScribeRestore(bool editing)
    {
        if (!editing)
        {
            PGuildScribeShow(false);
            return;
        }

        if (_pGuildVista?.LVistaStored is not long shown)
        {
            return;
        }

        PGuildScribeShow(true);
        PGuildMode.IsEnabled = true;
        PAutographOpen(shown);
    }

    internal void PGuildClear()
    {
        PAutographCancel();

        _pGuildVista?.LVistaSelect(null);
        PRollFind();
        PColophonReferenceHide();
        POeuvreFind();

        PVitaClear();
        PGuildScribeShow(false);
        PGuildMode.IsEnabled = false;
        PGuildBin.IsEnabled = false;
    }
}
