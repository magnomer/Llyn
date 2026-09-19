using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PYunjing
{
    private const string PYunjingFailure = "Yunjing.LoadFailed";

    private readonly ObservableCollection<PYunjingItem> _pShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _pYunmuList = [];

    private string? _pYunjingLanguage;

    private LVista? _pShengmuVista;

    private LVista? _pYunmuVista;

    private async void PYunjingWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PYunjingReset();
    }

    private void PYunjingLoad()
    {
        if (_pYunjingLanguage is not string language || _pShengmuVista is null || _pYunmuVista is null)
        {
            return;
        }

        IReadOnlyList<LDiwei> initials;
        IReadOnlyList<LDiwei> rimes;
        try
        {
            initials = _lEngine.LEngineDiweiFind(_pShengmuVista, language, LDiwei.LDiweiInitial);
            rimes = _lEngine.LEngineDiweiFind(_pYunmuVista, language, LDiwei.LDiweiRime);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
            return;
        }

        PYunjingListBuild(_pShengmuList, initials);
        PYunjingListBuild(_pYunmuList, rimes);
        PYunjingEmptyShow(
            PShengmuEmpty, _pShengmuList.Count, PPlumb.Text, "Yunjing.ShengmuEmpty", "Yunjing.ShengmuUnmatched");
        PYunjingEmptyShow(
            PYunmuEmpty, _pYunmuList.Count, PFathom.Text, "Yunjing.YunmuEmpty", "Yunjing.YunmuUnmatched");
        PXiaoyunFind();
        PDiweiLoad();
    }

    private static void PYunjingListBuild(ObservableCollection<PYunjingItem> list, IReadOnlyList<LDiwei> rows)
    {
        list.Clear();
        foreach (LDiwei row in rows)
        {
            list.Add(new PYunjingItem(row.LDiweiId, row.LDiweiKey, row.LDiweiCount)
            {
                PYunjingItemChosen = row.LDiweiChosen,
            });
        }
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

        bool onset = _pShengmuList.Contains(item);
        LVista? vista = onset ? _pShengmuVista : _pYunmuVista;
        if (vista is null)
        {
            return;
        }

        long? chosen = vista.LVistaMatch(item.PYunjingItemId) ? null : item.PYunjingItemId;
        vista.LVistaSelect(chosen);
        foreach (PYunjingItem listed in onset ? _pShengmuList : _pYunmuList)
        {
            listed.PYunjingItemChosen = vista.LVistaMatch(listed.PYunjingItemId);
        }

        PXiaoyunFind();
        if (chosen is null)
        {
            PDiweiHide();
        }
        else if (PDiweiFind(onset ? LDiwei.LDiweiInitial : LDiwei.LDiweiRime, item.PYunjingItemKey) is LDiwei diwei)
        {
            PDiweiShow(diwei);
        }
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
        PPlumb.Text = string.Empty;
        PFathom.Text = string.Empty;
        _pShengmuVista?.LVistaSelect(found.LDiweiFinal ? null : found.LDiweiId);
        _pYunmuVista?.LVistaSelect(found.LDiweiFinal ? found.LDiweiId : null);
        PYunjingLoad();
        PDiweiShow(found);
    }

    private void PPlumbHandle(object sender, TextChangedEventArgs e)
    {
        _pShengmuVista?.LVistaQuerySet(PPlumb.Text ?? string.Empty);
    }

    private void PFathomHandle(object sender, TextChangedEventArgs e)
    {
        _pYunmuVista?.LVistaQuerySet(PFathom.Text ?? string.Empty);
    }

    private void PBeaconHandle(object sender, TextChangedEventArgs e)
    {
        PXiaoyunFind();
    }

    private void PLadderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pShengmuVista is null)
        {
            return;
        }

        PLadderDropper.IsChecked = false;
        _pShengmuVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pShengmuVista.LVistaOrder));
    }

    private void PStairHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pYunmuVista is null)
        {
            return;
        }

        PStairDropper.IsChecked = false;
        _pYunmuVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pYunmuVista.LVistaOrder));
    }

    internal void PYunjingVistaRestore(LVista shengmu, LVista yunmu, LVista xiaoyun)
    {
        _pShengmuVista = shengmu;
        _pYunmuVista = yunmu;
        _pXiaoyunVista = xiaoyun;
        shengmu.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PYunjingLoad));
        yunmu.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PYunjingLoad));
        shengmu.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PYunjingWorkspaceUpdate));
        shengmu.LVistaObserverAttach(LSubject.LSubjectFanqie, new PObserver(this, PYunjingLoad));
        shengmu.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PDiweiLoad));
        shengmu.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PYunjingLoad));
        xiaoyun.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PXiaoyunEntryUpdate));
        xiaoyun.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PYunjingEntryUpdate));
        PDisplay.PDisplayVistaRestore(xiaoyun);
        PLadderRestore();
        PStairRestore();
        shengmu.LVistaQuerySet(PPlumb.Text ?? string.Empty);
        yunmu.LVistaQuerySet(PFathom.Text ?? string.Empty);
        PYunjingLoad();
    }

    private void PLadderRestore()
    {
        if (_pShengmuVista is not null)
        {
            PChoice.PChoiceOrderApply(PLadderDropdown, _pShengmuVista.LVistaOrder);
        }
    }

    private void PStairRestore()
    {
        if (_pYunmuVista is not null)
        {
            PChoice.PChoiceOrderApply(PStairDropdown, _pYunmuVista.LVistaOrder);
        }
    }
}
