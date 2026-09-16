using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PReference
{
    private static readonly IReadOnlyList<LAuthor> PShelfNobody = [];

    private readonly ObservableCollection<PShelfItem> _pShelfList = [];

    private IReadOnlyDictionary<long, int> _pShelfCount = new Dictionary<long, int>();

    private IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> _pShelfCredit =
        new Dictionary<long, IReadOnlyList<LAuthor>>();

    private long? _pColophonReference;

    private LCatalogOrder _pGradeChoice;

    private LCatalogFilter _pTrellisChoice = LCatalogFilter.LCatalogFilterEmpty;

    private void PReferenceBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
        {
            PImprint.PImprintBulletinHandle(bulletin);
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            PReferenceReset();
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectEntry
            && bulletin.LBulletinId > 0
            && _pDisplayEntry is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = bulletin.LBulletinId;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PImprint.PImprintBulletinHandle(bulletin);
        PShelfFind(PSurvey.Text ?? string.Empty);
        PFootnoteEntryUpdate(bulletin.LBulletinId);
    }

    private void PSurveyHandle(object sender, TextChangedEventArgs e)
    {
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    private void PRummageHandle(object sender, TextChangedEventArgs e)
    {
        PFootnoteFind();
    }

    private void PTrellisHandle(object sender, RoutedEventArgs e)
    {
        _pTrellisChoice = PChoice.PChoiceFilterRead(PTrellisList);
        _lEngine.LEngineTrellisSave(_pTrellisChoice);
        PTrellisMark.Visibility = _pTrellisChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PFootnoteFind();
    }

    internal async void PTrellisRestore(LCatalogFilter filter)
    {
        _pTrellisChoice = filter;
        PTrellisMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PTrellisList, _lEngine.LEngineLanguageRead(), filter, PTrellisHandle);
    }

    private void PGradeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pGradeChoice = LCatalog.LCatalogOrderParse(choice, _pGradeChoice);
        _lEngine.LEngineGradeSave(_pGradeChoice);
        PGradeDropper.IsChecked = false;
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    internal void PGradeRestore(LCatalogOrder order)
    {
        _pGradeChoice = order;
        PChoice.PChoiceOrderApply(PGradeDropdown, order);

        PImprint.PAuthorFind();
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    internal IReadOnlyList<LAuthor> PShelfCreditRead(long id)
    {
        return _pShelfCredit.TryGetValue(id, out IReadOnlyList<LAuthor>? credits) ? credits : PShelfNobody;
    }

    internal int PShelfCountRead(long id)
    {
        return _pShelfCount.TryGetValue(id, out int usage) ? usage : 0;
    }

    private void PShelfSelect(long? id)
    {
        foreach (PShelfItem item in _pShelfList)
        {
            item.PShelfItemChosen = id is not null
                && item.PShelfItemId == id;
        }
    }

    private void PShelfFind(string query)
    {
        IReadOnlyList<LCatalogReference> read;
        try
        {
            read = _lEngine.LEngineReferenceFind(query, _pGradeChoice);
            _pShelfCount = _lEngine.LEngineUsageRead(LOwner.LOwnerReference);
            _pShelfCredit = _lEngine.LEngineAuthorRead(LOwner.LOwnerReference);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        string unknown = _pReferenceHost.PLocalizationTextRead("Display.Unknown");
        string unset = _pReferenceHost.PLocalizationTextRead("Source.Unset");

        _pShelfList.Clear();
        bool kept = false;
        foreach (LCatalogReference row in read)
        {
            kept |= row.LCatalogReferenceStored.LReferenceId == _pColophonReference;
            _pShelfList.Add(new PShelfItem(row, unknown, unset));
        }

        PShelfEmpty.Visibility = _pShelfList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PShelfSelect(_pColophonReference);
        PColophon.PColophonTallyShow(PReferenceTallyRead(_pColophonReference));
        PImprint.PImprintTallyShow();

        if (!kept && _pColophonReference is not null && PImprint.Visibility != Visibility.Visible)
        {
            PReferenceClear();
        }

        PFootnoteFind();
    }

    private void PShelfHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PShelfItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PReferenceShow(item.PShelfItemId);
    }

    internal void PReferenceShow(long id)
    {
        LReference? reference;
        try
        {
            reference = _lEngine.LEngineReferenceRead(id);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        if (reference is null)
        {
            PReferenceClear();
            PShelfFind(PSurvey.Text ?? string.Empty);
            return;
        }

        _pColophonReference = id;
        PShelfSelect(id);
        PFootnoteEntryHide();

        PColophon.PColophonShow(reference, PShelfCreditRead(id), PReferenceTallyRead(id));

        PFootnoteFind();

        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = true;

        if (PImprint.Visibility == Visibility.Visible)
        {
            PImprint.PImprintDraftOpen(id);
        }
    }

    private void PReferenceBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is not null || _pColophonReference is not long id)
        {
            return;
        }

        int usage = PShelfCountRead(id);

        if (!_pReferenceHost.PWindowRemovalConfirm(usage, "Source"))
        {
            return;
        }

        try
        {
            _lEngine.LEngineReferenceDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.DeleteFailed", exception);
            return;
        }

        PReferenceScribeShow(false);
        PReferenceClear();
    }

    internal string PReferenceTallyRead(long? id)
    {
        int count = id is long stored ? PShelfCountRead(stored) : 0;

        return count switch
        {
            0 => _pReferenceHost.PLocalizationTextRead("Source.UsageNone"),
            1 => _pReferenceHost.PLocalizationTextRead("Source.UsageOne"),
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} "
                + _pReferenceHost.PLocalizationTextRead("Source.UsageMany"),
        };
    }

    private void PReferenceScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PReferenceScribe);
        if (_pDisplayEntry is not null || PEditor.Visibility == Visibility.Visible)
        {
            PFootnoteScribeHandle(editing);
            return;
        }

        if (editing == (PImprint.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PReferenceLeaveConfirm())
            {
                PReferenceScribeShow(true);
                return;
            }

            PImprint.PImprintDraftCancel();
            PReferenceScribeShow(false);

            if (_pColophonReference is not null)
            {
                PReferenceShow(_pColophonReference.Value);
                return;
            }

            PReferenceClear();
            return;
        }

        if (_pColophonReference is null)
        {
            PReferenceClear();
            return;
        }

        PReferenceScribeShow(true);
        PImprint.PImprintDraftOpen(_pColophonReference);
    }

    internal void PReferenceScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PImprint.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PColophon.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PReferenceViewer.IsChecked = !editing;
        PReferenceScribe.IsChecked = editing;
        PImprint.PImprintChangeUpdate();
    }

    internal void PReferenceScribeRestore(bool editing)
    {
        if (editing && _pColophonReference is null)
        {
            return;
        }

        if (editing)
        {
            PReferenceMode.IsEnabled = true;
        }

        PReferenceScribeShow(editing);
    }

    internal bool PReferenceLeaveConfirm()
    {
        return _pReferenceHost.PWindowDiscardConfirm(PReferenceChangeCheck(), PReferenceDraftFinish);
    }

    internal void PReferenceClear()
    {
        PImprint.PImprintDraftCancel();

        _pColophonReference = null;
        PShelfSelect(null);
        PFootnoteEntryHide();
        PFootnoteFind();

        PColophon.PColophonClear();
        PImprint.PImprintClear();
        PReferenceScribeShow(false);
        PReferenceMode.IsEnabled = false;
        PReferenceBin.IsEnabled = false;
    }
}
