using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PWing : UserControl
{
    private readonly ObservableCollection<PIndexItem> _pWingIndex = [];

    private PWindow _pWingHost = null!;

    private LEngine _lEngine = null!;

    private LVista? _pWingVista;

    private PObserver? _pWingObserver;

    public PWing()
    {
        InitializeComponent();
    }

    internal void PWingAttach(PWindow host, LEngine engine)
    {
        _pWingHost = host;
        _lEngine = engine;

        PWingIndex.ItemsSource = _pWingIndex;

        _pWingObserver = new PObserver(this, PWingBulletinHandle);
        engine.LEngineObserverAttach(_pWingObserver);

        PWingDisplay.PDisplayAttach(host, engine);
    }

    internal async void PWingRestore(LVista vista, long? id)
    {
        _pWingVista = vista;
        _pWingIndex.Clear();
        await PEnsign.PEnsignLoad(_lEngine);

        PWingOrderRestore();
        PWingSieveRestore();
        PChoice.PChoiceFilterBuild(
            PWingSieveList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PWingSieveHandle);

        PWingQuery.Text = string.Empty;
        PWingDisplay.PDisplayClear();

        if (id is long shown)
        {
            PWingEntryShow(shown);
        }
    }

    internal void PWingClose()
    {
        if (_pWingObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pWingObserver);
            _pWingObserver = null;
        }

        PWingDisplay.PDisplayClose();
    }

    private void PWingBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
        {
            if (_pWingVista is not null && bulletin.LBulletinId == _pWingVista.LVistaId)
            {
                PWingIndexFind();
            }

            return;
        }

        if (PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            PWingIndexFind();
        }
    }

    private void PWingOrderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pWingVista is null)
        {
            return;
        }

        PWingOrderDropper.IsChecked = false;
        _pWingVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pWingVista.LVistaOrder));
    }

    private void PWingSieveHandle(object sender, RoutedEventArgs e)
    {
        if (_pWingVista is null)
        {
            return;
        }

        _pWingVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PWingSieveList));
        PWingSieveRestore();
    }

    private void PWingQueryHandle(object sender, TextChangedEventArgs e)
    {
        string query = PWingQuery.Text ?? string.Empty;
        PWingIndex.Visibility = query.Trim().Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        _pWingVista?.LVistaQuerySet(query);
    }

    private void PWingKeyHandle(object sender, KeyEventArgs e)
    {
        if (_pWingVista is null || PWingIndex.Visibility != Visibility.Visible)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            PWingIndex.Visibility = Visibility.Collapsed;
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            if (_pWingVista.LVistaChosen is long chosen && _pWingIndex.Any(row => row.PIndexItemId == chosen))
            {
                PWingIndex.Visibility = Visibility.Collapsed;
                PWingEntryShow(chosen);
                PWingEntrySave();
            }

            e.Handled = true;
            return;
        }

        if (e.Key is not (Key.Down or Key.Up) || _pWingIndex.Count == 0)
        {
            return;
        }

        int place = -1;
        for (int index = 0; index < _pWingIndex.Count; index++)
        {
            if (_pWingIndex[index].PIndexItemChosen)
            {
                place = index;
                break;
            }
        }

        place = e.Key == Key.Down
            ? Math.Min(place + 1, _pWingIndex.Count - 1)
            : Math.Max(place - 1, 0);
        PIndexItem target = _pWingIndex[place];
        _pWingVista.LVistaSelect(target.PIndexItemId);
        PWingChosenApply();
        if (PWingIndex.ItemContainerGenerator.ContainerFromItem(target) is FrameworkElement container)
        {
            container.BringIntoView();
        }

        e.Handled = true;
    }

    private void PWingLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus is Visual target
            && (ReferenceEquals(target, PWingQuery) || PWingIndex.IsAncestorOf(target)))
        {
            return;
        }

        PWingIndex.Visibility = Visibility.Collapsed;
    }

    private void PWingIndexHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PIndexItem item })
        {
            return;
        }

        PWingIndex.Visibility = Visibility.Collapsed;
        PWingEntryShow(item.PIndexItemId);
        PWingEntrySave();
    }

    private void PWingOrderRestore()
    {
        if (_pWingVista is not null)
        {
            PChoice.PChoiceOrderApply(PWingOrderDropdown, _pWingVista.LVistaOrder);
        }
    }

    private void PWingSieveRestore()
    {
        bool active = _pWingVista?.LVistaFilter.LCatalogFilterActive == true;
        PWingSieveMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PWingChosenApply()
    {
        long? chosen = _pWingVista?.LVistaChosen;
        foreach (PIndexItem item in _pWingIndex)
        {
            item.PIndexItemChosen = chosen is not null
                && item.PIndexItemId == chosen;
        }
    }

    private void PWingIndexFind()
    {
        _pWingIndex.Clear();
        if (_pWingVista is null)
        {
            return;
        }

        foreach (LVistaRow row in _lEngine.LEngineEntryFind(_pWingVista))
        {
            _pWingIndex.Add(new PIndexItem(
                row.LVistaRowId,
                row.LVistaRowHeadword,
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty)
            {
                PIndexItemName = row.LVistaRowName,
                PIndexItemChosen = row.LVistaRowChosen,
            });
        }

        bool typed = _pWingVista.LVistaQuery.Trim().Length > 0;
        PWingEmpty.Visibility = typed && _pWingIndex.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PWingEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pWingHost.PWindowFailureShow("Duplex.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            _pWingVista?.LVistaSelect(null);
            PWingChosenApply();
            PWingDisplay.PDisplayClear();
            return;
        }

        _pWingVista?.LVistaSelect(id);
        PWingChosenApply();
        PWingDisplay.PDisplayShow(id, draft);
    }

    private void PWingEntrySave()
    {
        long? shown = _pWingVista?.LVistaChosen;
        if (string.Equals(_pWingVista?.LVistaTab, "left", StringComparison.Ordinal))
        {
            _lEngine.LEngineLeftSave(shown);
            return;
        }

        _lEngine.LEngineRightSave(shown);
    }
}
