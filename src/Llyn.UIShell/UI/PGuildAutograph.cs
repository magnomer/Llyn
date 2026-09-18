using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PGuild
{
    private const int PAutographUnionLimit = 8;

    private readonly ObservableCollection<PRollItem> _pAutographUnion = [];

    private LTenure? _pAutographTenure;

    private void PAutographOpen(long? id)
    {
        PAutographCancel();

        string name = string.Empty;
        try
        {
            _pAutographTenure = _lEngine.LEngineTenureStart("guild", LSubject.LSubjectAuthor, id);
            _pAutographTenure.LTenureDraftAttach(LSubject.LSubjectDraft, new PObserver(this, PAutographChangeUpdate));
            _pAutographTenure.LTenureDraftAttach(LSubject.LSubjectTenure, new PObserver(this, PAutographChangeUpdate));
            name = _pAutographTenure.LTenureRead()?.LDraftAuthorHeld?.LAuthorName ?? string.Empty;
        }
        catch (Exception exception)
        {
            _pAutographTenure?.LTenureCancel();
            _pAutographTenure = null;
            _pGuildHost.PWindowFailureShow("Guild.LoadFailed", exception);
        }

        PAutographName.Text = name;
        PAutographTallyShow(id ?? 0);

        PAutographUnion.Text = string.Empty;
        _pAutographUnion.Clear();
        PAutographUnionBody.Visibility = id is null ? Visibility.Collapsed : Visibility.Visible;
        PAutographUnionNotice.Visibility = id is null ? Visibility.Visible : Visibility.Collapsed;

        PAutographChangeUpdate();
        PAutographName.Focus();
    }

    private void PAutographCancel()
    {
        if (_pAutographTenure is LTenure held)
        {
            _pAutographTenure = null;
            held.LTenureFinish(false);
        }

        PAutographName.Text = string.Empty;
        PAutographUnion.Text = string.Empty;
        _pAutographUnion.Clear();
        PAutographChangeUpdate();
    }

    private void PAutographTallyShow(long id)
    {
        LCatalogAuthor? row = id > 0 ? PVitaCatalogRead(id) : null;

        PAutographWork.Text = PGuildWorkRead(row?.LCatalogAuthorWork ?? 0);
        PAutographTally.Text = PGuildTallyRead(row?.LCatalogAuthorUsage ?? 0);
    }

    private bool PAutographChangeCheck()
    {
        return _pAutographTenure?.LTenureStateRead().LTenureStateChanged == true;
    }

    private void PAutographChangeUpdate()
    {
        string typed = (PAutographName.Text ?? string.Empty).Trim();
        PGuildStore.IsEnabled = PAutograph.Visibility == Visibility.Visible
            && typed.Length > 0
            && PAutographChangeCheck();
    }

    private void PAutographNameHandle(object sender, TextChangedEventArgs e)
    {
        if (_pAutographTenure is LTenure held)
        {
            held.LTenureRequestDefer(new LRequestAuthorName(held.LTenureId, PAutographName.Text ?? string.Empty));
        }

        PAutographChangeUpdate();
    }

    private bool PAutographStoreRun()
    {
        string typed = (PAutographName.Text ?? string.Empty).Trim();
        if (typed.Length == 0)
        {
            _pGuildHost.PWindowFailureShow("Guild.NameBlank");
            return false;
        }

        if (_pAutographTenure is not LTenure held)
        {
            return true;
        }

        long? stored;
        try
        {
            stored = held.LTenureFinish(true);
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.SaveFailed", exception);
            return false;
        }

        _pAutographTenure = null;
        long? shown = _pGuildVista?.LVistaChosen is long chosen && chosen > 0 ? chosen : null;
        if (stored is long author && author != shown)
        {
            _pGuildVista?.LVistaSelect(author);
            PRollFind();
            PGuildMode.IsEnabled = true;
            PGuildBin.IsEnabled = true;
            shown = author;
        }

        PAutographOpen(shown);
        return true;
    }

    private void PAutographUnionHandle(object sender, TextChangedEventArgs e)
    {
        string word = (PAutographUnion.Text ?? string.Empty).Trim();
        _pAutographUnion.Clear();

        if (word.Length == 0 || _pGuildVista?.LVistaChosen is not long self || self <= 0)
        {
            return;
        }

        IReadOnlyList<LCatalogAuthor> found;
        try
        {
            found = _lEngine.LEngineAuthorFind(word, LCatalogOrder.LCatalogOrderName);
        }
        catch (Exception)
        {
            return;
        }

        foreach (LCatalogAuthor row in found)
        {
            if (row.LCatalogAuthorStored.LAuthorId == self)
            {
                continue;
            }

            _pAutographUnion.Add(new PRollItem(row, PGuildWorkRead(row.LCatalogAuthorWork), false));

            if (_pAutographUnion.Count == PAutographUnionLimit)
            {
                break;
            }
        }
    }

    private void PAutographUnionSelect(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row
            || row.DataContext is not PRollItem item
            || _pGuildVista?.LVistaChosen is not long dropped
            || dropped <= 0)
        {
            return;
        }

        if (!_pGuildHost.PWindowUnionConfirm(PAutographName.Text ?? string.Empty, item.PRollItemName))
        {
            return;
        }

        long kept = item.PRollItemId;
        try
        {
            _lEngine.LEngineAuthorAbsorb(kept, dropped);
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.MergeFailed", exception);
            return;
        }

        PAutographCancel();
        PGuildScribeShow(false);
        PRollAuthorShow(kept);
    }

    private void PGuildFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PGuildLeaveConfirm())
        {
            return;
        }

        PGuildClear();
        PGuildScribeShow(true);
        PAutographOpen(null);
        PGuildMode.IsEnabled = true;
    }

    private void PGuildStoreHandle(object sender, RoutedEventArgs e)
    {
        PAutographStoreRun();
    }

    private void PGuildBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pGuildVista?.LVistaChosen is not long id || id <= 0 || _pOeuvreVista?.LVistaChosen is not null)
        {
            return;
        }

        int works = PVitaCatalogRead(id)?.LCatalogAuthorWork ?? 0;

        if (!_pGuildHost.PWindowRemovalConfirm(works, "Guild"))
        {
            return;
        }

        try
        {
            _pGuildVista.LVistaDelete();
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.DeleteFailed", exception);
            return;
        }

        PGuildScribeShow(false);
        PGuildClear();
    }
}
