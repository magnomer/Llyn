using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PWing : UserControl
{
    private readonly ObservableCollection<PIndexItem> _pWingIndex = [];

    private PWindow _pWingHost = null!;

    private LWing _lWing = null!;

    public PWing()
    {
        InitializeComponent();
    }

    internal void PWingAttach(PWindow host, LEngine engine)
    {
        _pWingHost = host;
        _lWing = new LWing(engine, engine, engine);

        PWingIndex.ItemsSource = _pWingIndex;

        PWingDisplay.PDisplayAttach(host, _lWing.LWingDisplay);
    }

    internal async void PWingRestore(string tab, long? id)
    {
        _lWing.LWingVistaRestore(_pWingHost.PWindowPosture, tab);
        _lWing.LWingObserverAttach(LSubject.LSubjectVista, new PObserver(this, PWingIndexFind));
        _lWing.LWingObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PWingIndexFind));
        _lWing.LWingObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PWingIndexFind));
        _lWing.LWingObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PWingIndexFind));
        PWingDisplay.PDisplayObserverAttach();
        _pWingIndex.Clear();
        await PEnsign.PEnsignLoad(_pWingHost.PWindowDeportment);

        PChoice.PChoiceOrderApply(PWingOrderDropdown, _lWing.LWingOrder);
        PWingSieveRestore();
        PChoice.PChoiceFilterBuild(
            PWingSieveList, _lWing.LWingLanguageRead(), _lWing.LWingFilter, PWingSieveHandle);

        PWingQuery.Text = string.Empty;
        PWingDisplay.PDisplayClear();

        if (id is long shown)
        {
            PWingEntryShow(shown);
        }
    }

    internal void PWingClose()
    {
        PWingDisplay.PDisplayClose();
    }

    private void PWingOrderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        PWingOrderDropper.IsChecked = false;
        _lWing.LWingOrderSet(choice);
    }

    private void PWingSieveHandle(object sender, RoutedEventArgs e)
    {
        _lWing.LWingSieveSet(PChoice.PChoiceFilterRead(PWingSieveList));
        PWingSieveRestore();
    }

    private void PWingQueryHandle(object sender, TextChangedEventArgs e)
    {
        string query = PWingQuery.Text ?? string.Empty;
        PWingIndex.Visibility = query.Trim().Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        _lWing.LWingQuerySet(query);
    }

    private void PWingKeyHandle(object sender, KeyEventArgs e)
    {
        if (PWingIndex.Visibility != Visibility.Visible)
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
            if (_lWing.LWingChosen is long chosen)
            {
                if (_pWingIndex.Any(row => row.PIndexItemId == chosen))
                {
                    PWingIndex.Visibility = Visibility.Collapsed;
                    PWingEntryShow(chosen);
                    PWingEntrySave();
                }
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
        _lWing.LWingSelect(target.PIndexItemId);
        PWingIndexFind();
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

    private void PWingSieveRestore()
    {
        PWingSieveMark.Visibility = _lWing.LWingFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PWingIndexFind()
    {
        List<PIndexItem> fresh = [];
        IReadOnlyList<LVistaRow> read = _lWing.LWingRowsRead();
        foreach (LVistaRow row in read)
        {
            fresh.Add(new PIndexItem(
                row.LVistaRowId,
                row.LVistaRowHeadword,
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty,
                row.LVistaRowChosen)
            {
                PIndexItemName = row.LVistaRowName,
            });
        }

        PSplice.PSpliceApply(_pWingIndex, fresh, PIndexItem.PIndexItemMatch, PIndexItem.PIndexItemSync);
        bool typed = _lWing.LWingQueried;
        PWingEmpty.Visibility = typed && _pWingIndex.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PWingEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lWing.LWingEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pWingHost.PWindowFailureShow("Duplex.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            _lWing.LWingSelect(null);
            PWingIndexFind();
            PWingDisplay.PDisplayClear();
            return;
        }

        _lWing.LWingSelect(id);
        PWingIndexFind();
        PWingDisplay.PDisplayShow(draft);
    }

    private void PWingEntrySave()
    {
        _lWing.LWingEntrySave();
    }
}
