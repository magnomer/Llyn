using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PGuild : UserControl
{
    private PWindow _pGuildHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pGuildObserver;

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

        _pGuildObserver = new PObserver(this, PGuildBulletinHandle);
        engine.LEngineObserverAttach(_pGuildObserver);
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
        if (_pGuildObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pGuildObserver);
            _pGuildObserver = null;
        }

        PEchelonDropdown.IsOpen = false;
        PLouverDropdown.IsOpen = false;
    }

    internal bool PGuildLeaveConfirm()
    {
        return _pGuildHost.PWindowDiscardConfirm(PGuildChangeCheck(), PGuildDraftFinish);
    }

    private void PGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pColophonReference is not null && PColophon.Visibility == Visibility.Visible;
    }

    private async void PGuildPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pColophonReference is long shown && PColophon.Visibility == Visibility.Visible)
        {
            await _pGuildHost.PWindowPressRun(ticket => _lEngine.LEnginePortraitPrint(
                shown, LOwner.LOwnerReference, _pGuildHost.PWindowLegendRead("Source"), ticket));
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

            if (_pRollAuthor is long kept && kept > 0)
            {
                PVitaShow(kept);
                return;
            }

            PGuildClear();
            return;
        }

        if (_pRollAuthor is not long author || author <= 0)
        {
            PGuildClear();
            return;
        }

        PGuildScribeShow(true);
        PAutographOpen(author);
    }

    internal void PGuildScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PAutograph.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PVita.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PGuildViewer.IsChecked = !editing;
        PGuildScribe.IsChecked = editing;
        PAutographChangeUpdate();
    }

    internal void PGuildScribeRestore(bool editing)
    {
        if (editing && (_pRollAuthor is not long author || author <= 0))
        {
            return;
        }

        PGuildScribeShow(editing);

        if (editing && _pRollAuthor is long shown)
        {
            PGuildMode.IsEnabled = true;
            PAutographOpen(shown);
        }
    }

    internal void PGuildClear()
    {
        PAutographCancel();

        _pRollAuthor = null;
        PRollSelect(null);
        PColophonReferenceHide();
        POeuvreFind();

        PVitaClear();
        PGuildScribeShow(false);
        PGuildMode.IsEnabled = false;
        PGuildBin.IsEnabled = false;
    }
}
