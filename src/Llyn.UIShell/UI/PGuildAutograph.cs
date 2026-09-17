using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PGuild
{
    private const int PAutographUnionLimit = 8;

    private readonly ObservableCollection<PRollItem> _pAutographUnion = [];

    private long? _pAutographDraft;

    private string _pAutographStored = string.Empty;

    private void PAutographOpen(long? id)
    {
        _pAutographDraft = id;

        string name = string.Empty;
        if (id is long stored)
        {
            LAuthor? author;
            try
            {
                author = _lEngine.LEngineAuthorRead(stored);
            }
            catch (Exception exception)
            {
                _pGuildHost.PWindowFailureShow("Guild.LoadFailed", exception);
                author = null;
            }

            name = author?.LAuthorName ?? string.Empty;
        }

        _pAutographStored = name.Trim();
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
        _pAutographDraft = null;
        _pAutographStored = string.Empty;
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
        string typed = (PAutographName.Text ?? string.Empty).Trim();
        return !string.Equals(typed, _pAutographStored, StringComparison.Ordinal);
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

        try
        {
            if (_pAutographDraft is long stored)
            {
                _lEngine.LEngineAuthorUpdate(new LAuthor(stored, typed));
                _pAutographStored = typed;
                PAutographChangeUpdate();
                return true;
            }

            LAuthor created = _lEngine.LEngineAuthorCreate(new LAuthor(0, typed));
            _pRollAuthor = created.LAuthorId;
            PRollFind();
            PGuildMode.IsEnabled = true;
            PGuildBin.IsEnabled = true;
            PAutographOpen(created.LAuthorId);
            return true;
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.SaveFailed", exception);
            return false;
        }
    }

    private void PAutographUnionHandle(object sender, TextChangedEventArgs e)
    {
        string word = (PAutographUnion.Text ?? string.Empty).Trim();
        _pAutographUnion.Clear();

        if (word.Length == 0 || _pAutographDraft is not long self)
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

            _pAutographUnion.Add(new PRollItem(row, PGuildWorkRead(row.LCatalogAuthorWork)));

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
            || _pAutographDraft is not long dropped)
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
        if (_pRollAuthor is not long id || id <= 0 || _pColophonReference is not null)
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
            _lEngine.LEngineAuthorDelete(id, works > 0);
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
