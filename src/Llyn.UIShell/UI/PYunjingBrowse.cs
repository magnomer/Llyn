using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PYunjing
{
    private const string PYunjingFailure = "Yunjing.LoadFailed";

    private readonly ObservableCollection<PYunjingItem> _pShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _pYunmuList = [];

    private string? _pYunjingLanguage;

    private long? _pShengmuChoice;

    private long? _pYunmuChoice;

    private LCatalogOrder _pLadderChoice;

    private LCatalogOrder _pStairChoice;

    private async void PYunjingBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PYunjingReset();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectFanqie)
        {
            PYunjingLoad();
            return;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PXiaoyunEntryUpdate(bulletin.LBulletinId);
    }

    private void PYunjingLoad()
    {
        if (_pYunjingLanguage is not string language)
        {
            return;
        }

        IReadOnlyList<LDiwei> initials;
        IReadOnlyList<LDiwei> rimes;
        try
        {
            initials = _lEngine.LEngineDiweiRead(language, LDiwei.LDiweiInitial);
            rimes = _lEngine.LEngineDiweiRead(language, LDiwei.LDiweiRime);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
            return;
        }

        _pShengmuChoice = PYunjingListBuild(
            _pShengmuList, initials, _pShengmuChoice, PPlumb.Text ?? string.Empty, _pLadderChoice);
        _pYunmuChoice = PYunjingListBuild(
            _pYunmuList, rimes, _pYunmuChoice, PFathom.Text ?? string.Empty, _pStairChoice);
        PYunjingEmptyShow(
            PShengmuEmpty, _pShengmuList.Count, PPlumb.Text, "Yunjing.ShengmuEmpty", "Yunjing.ShengmuUnmatched");
        PYunjingEmptyShow(
            PYunmuEmpty, _pYunmuList.Count, PFathom.Text, "Yunjing.YunmuEmpty", "Yunjing.YunmuUnmatched");
        PXiaoyunFind();
        PDiweiLoad();
    }

    private static long? PYunjingListBuild(
        ObservableCollection<PYunjingItem> list,
        IReadOnlyList<LDiwei> rows,
        long? chosen,
        string query,
        LCatalogOrder order)
    {
        string wanted = query.Trim();
        IEnumerable<LDiwei> kept = rows.Where(row =>
            wanted.Length == 0 || row.LDiweiKey.Contains(wanted, StringComparison.OrdinalIgnoreCase));
        kept = order switch
        {
            LCatalogOrder.LCatalogOrderReverse => kept.OrderByDescending(row => row.LDiweiKey, StringComparer.Ordinal),
            LCatalogOrder.LCatalogOrderUsage => kept
                .OrderByDescending(row => row.LDiweiCount)
                .ThenBy(row => row.LDiweiKey, StringComparer.Ordinal),
            _ => kept.OrderBy(row => row.LDiweiKey, StringComparer.Ordinal),
        };

        list.Clear();
        bool held = false;
        foreach (LDiwei row in kept)
        {
            bool marked = row.LDiweiId == chosen;
            held |= marked;
            list.Add(new PYunjingItem(row.LDiweiId, row.LDiweiKey, row.LDiweiCount) { PYunjingItemChosen = marked });
        }

        return held ? chosen : null;
    }

    private static void PYunjingEmptyShow(TextBlock label, int count, string? query, string vacant, string unmatched)
    {
        label.SetResourceReference(TextBlock.TextProperty, string.IsNullOrWhiteSpace(query) ? vacant : unmatched);
        label.Visibility = count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PYunjingHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PYunjingItem item)
        {
            return;
        }

        if (_pShengmuList.Contains(item))
        {
            _pShengmuChoice = _pShengmuChoice == item.PYunjingItemId ? null : item.PYunjingItemId;
            PYunjingSelect(_pShengmuList, _pShengmuChoice);
            if (_pShengmuChoice is null)
            {
                PDiweiHide();
            }
            else if (PDiweiFind(LDiwei.LDiweiInitial, item.PYunjingItemKey) is LDiwei diwei)
            {
                PDiweiShow(diwei);
            }
        }
        else
        {
            _pYunmuChoice = _pYunmuChoice == item.PYunjingItemId ? null : item.PYunjingItemId;
            PYunjingSelect(_pYunmuList, _pYunmuChoice);
        }

        PXiaoyunFind();
    }

    internal void PYunjingDiweiShow(string language, string kind, string key)
    {
        LDiwei? found;
        try
        {
            found = _lEngine.LEngineDiweiFind(language, kind, key);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
            return;
        }

        if (found is null)
        {
            return;
        }

        _pYunjingLanguage = language;
        _pShengmuChoice = kind == LDiwei.LDiweiInitial ? found.LDiweiId : null;
        _pYunmuChoice = kind == LDiwei.LDiweiRime ? found.LDiweiId : null;
        PPlumb.Text = string.Empty;
        PFathom.Text = string.Empty;
        PYunjingLoad();
        if (kind == LDiwei.LDiweiInitial)
        {
            PDiweiShow(found);
        }
    }

    private static void PYunjingSelect(ObservableCollection<PYunjingItem> list, long? chosen)
    {
        foreach (PYunjingItem item in list)
        {
            item.PYunjingItemChosen = chosen is not null && item.PYunjingItemId == chosen;
        }
    }

    private void PPlumbHandle(object sender, TextChangedEventArgs e)
    {
        PYunjingLoad();
    }

    private void PFathomHandle(object sender, TextChangedEventArgs e)
    {
        PYunjingLoad();
    }

    private void PBeaconHandle(object sender, TextChangedEventArgs e)
    {
        PXiaoyunFind();
    }

    private void PLadderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pLadderChoice = LCatalog.LCatalogOrderParse(choice, _pLadderChoice);
        _lEngine.LEngineLadderSave(_pLadderChoice);
        PLadderDropper.IsChecked = false;
        PYunjingLoad();
    }

    private void PStairHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pStairChoice = LCatalog.LCatalogOrderParse(choice, _pStairChoice);
        _lEngine.LEngineStairSave(_pStairChoice);
        PStairDropper.IsChecked = false;
        PYunjingLoad();
    }

    internal void PLadderRestore(LCatalogOrder order)
    {
        _pLadderChoice = order;
        PChoice.PChoiceOrderApply(PLadderDropdown, order);
        PYunjingLoad();
    }

    internal void PStairRestore(LCatalogOrder order)
    {
        _pStairChoice = order;
        PChoice.PChoiceOrderApply(PStairDropdown, order);
        PYunjingLoad();
    }
}
