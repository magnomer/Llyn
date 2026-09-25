using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LWing
{
    private readonly LEntryPort _lEntryPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lWingVista;

    private LIndex? _lWingIndex;

    private TextBox? _lWingQuery;

    public LWing(LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);

        _lEntryPort = entries;
        _lSettingsPort = settings;
        LWingDisplay = new LDisplay(entries, phonology, settings);
    }

    public event Action<string, Exception>? LWingFailed;

    public event Action<LEntryDraft>? LWingDraftChanged;

    public event Action? LWingCleared;

    public LDisplay LWingDisplay { get; }

    private bool LWingFiltered => _lWingVista?.LVistaFiltered ?? false;

    private bool LWingQueried => _lWingVista?.LVistaQueried ?? false;

    private bool LWingShown => _lWingIndex?.LIndexShown ?? false;

    private void LWingVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lWingVista = vista;
        LWingDisplay.LDisplayVistaRestore(vista);
    }

    private void LWingQuerySet(string query)
    {
        _lWingVista?.LVistaQuerySet(query);
    }

    private void LWingOrderSet(LCatalogOrder? order)
    {
        if (_lWingVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    private void LWingSieveSet(LCatalogFilter filter)
    {
        _lWingVista?.LVistaFilterSet(filter);
    }

    public void LWingOrderHandle(object sender, ToggleButton dropper)
    {
        ArgumentNullException.ThrowIfNull(dropper);

        dropper.IsChecked = false;
        LWingOrderSet(LChoice.LChoiceOrderRead(sender));
    }

    public void LWingSieveHandle(Panel list, UIElement mark)
    {
        LWingSieveSet(LChoice.LChoiceFilterRead(list));
        LWingSieveShow(mark);
    }

    public void LWingSieveShow(UIElement mark)
    {
        ArgumentNullException.ThrowIfNull(mark);

        mark.Visibility = LWingFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void LWingSelect(long? id)
    {
        _lWingVista?.LVistaSelect(id);
    }

    public void LWingVistaRestore(LWindow window, string tab)
    {
        ArgumentNullException.ThrowIfNull(window);

        LWingVistaRestore(
            window.LWindowVistaStart(tab, LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword, true));
    }

    public void LWingIndexAttach(
        ItemsControl view, FrameworkElement empty, TextBox query, Func<string, ImageSource?> flagSeam)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lWingIndex = new LIndex(view, empty, flagSeam);
        _lWingQuery = query;
    }

    public void LWingObserverAttach(
        DispatcherObject surface, Func<DispatcherObject, Action, Action<LBulletin>> observerSeam)
    {
        ArgumentNullException.ThrowIfNull(observerSeam);

        Action<LBulletin> observer = observerSeam(surface, LWingIndexShow);
        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectVista, observer);
        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectEntry, observer);
        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectReflex, observer);
        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectSettings, observer);
    }

    private void LWingIndexShow()
    {
        _lWingIndex?.LIndexShow(LWingRowsRead(), LWingQueried);
    }

    public void LWingIndexClear()
    {
        _lWingIndex?.LIndexClear();
    }

    private void LWingIndexHide()
    {
        _lWingIndex?.LIndexShownSet(false);
    }

    public void LWingEntryShow(long? id)
    {
        LWingCleared?.Invoke();
        if (id is long shown)
        {
            LWingEntryLoad(shown);
        }
    }

    private void LWingEntryLoad(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEntryPort.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            LWingFailed?.Invoke("Duplex.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            LWingSelect(null);
            LWingIndexShow();
            LWingCleared?.Invoke();
            return;
        }

        LWingSelect(id);
        LWingIndexShow();
        LWingDraftChanged?.Invoke(draft);
    }

    public void LWingQueryHandle()
    {
        LWingQuerySet(_lWingQuery?.Text ?? string.Empty);
        _lWingIndex?.LIndexShownSet(LWingQueried);
    }

    public void LWingKeyHandle(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        LIndex? index = _lWingIndex;
        if (!LWingShown)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            LWingIndexHide();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            if (index?.LIndexChosenRead() is long chosen)
            {
                LWingIndexHide();
                LWingEntryLoad(chosen);
                LWingEntrySave();
            }

            e.Handled = true;
            return;
        }

        if (e.Key is not (Key.Down or Key.Up) || index?.LIndexNeighbourFind(e.Key == Key.Down) is not long target)
        {
            return;
        }

        LWingSelect(target);
        LWingIndexShow();
        index?.LIndexEntryScroll(target);
        e.Handled = true;
    }

    public void LWingLeaveHandle(KeyboardFocusChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.NewFocus is Visual target
            && (ReferenceEquals(target, _lWingQuery) || (_lWingIndex?.LIndexHoldCheck(target) ?? false)))
        {
            return;
        }

        LWingIndexHide();
    }

    public void LWingIndexHandle(object sender)
    {
        if (LIndex.LIndexEntryRead(sender) is not long id)
        {
            return;
        }

        LWingIndexHide();
        LWingEntryLoad(id);
        LWingEntrySave();
    }

    public LCatalogOrder LWingOrder => _lWingVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LWingFilter => _lWingVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    private IReadOnlyList<LVistaRow> LWingRowsRead()
    {
        return _lWingVista is LVista vista ? _lEntryPort.LEngineEntryFind(vista) : [];
    }

    public IReadOnlyList<string> LWingLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    private void LWingEntrySave()
    {
        if (_lWingVista is LVista vista)
        {
            if (vista.LVistaLeft)
            {
                _lSettingsPort.LEngineLeftSave(vista.LVistaChosen);
                return;
            }
        }

        _lSettingsPort.LEngineRightSave(_lWingVista?.LVistaChosen);
    }
}
