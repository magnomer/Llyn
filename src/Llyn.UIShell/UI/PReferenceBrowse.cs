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

    private readonly ObservableCollection<PFootnoteItem> _pFootnoteList = [];

    private IReadOnlyDictionary<long, int> _pShelfCount = new Dictionary<long, int>();

    private IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> _pShelfCredit =
        new Dictionary<long, IReadOnlyList<LAuthor>>();

    private long? _pColophonReference;

    private long? _pDisplayEntry;

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
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} {_pReferenceHost.PLocalizationTextRead("Source.UsageMany")}",
        };
    }

    private void PFootnoteFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(
                new LReference(
                    _pColophonReference ?? 0,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LReferenceKind.LReferenceKindUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateMark.LStateMarkUnspecified),
                PRummage.Text ?? string.Empty,
                _pTrellisChoice);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        _pFootnoteList.Clear();
        foreach (LEntry entry in read)
        {
            _pFootnoteList.Add(new PFootnoteItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        PTwin.PTwinNameApply(
            _pFootnoteList,
            row => row.PFootnoteItemHeadword,
            (row, name) => row.PFootnoteItemName = name,
            row => row.PFootnoteItemId);

        PFootnoteEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PRummage.Text) ? "Source.Vacant" : "Source.Unmatched");
        PFootnoteEmpty.Visibility = _pFootnoteList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PFootnoteSelect(_pDisplayEntry);
    }

    private void PFootnoteSelect(long? id)
    {
        foreach (PFootnoteItem item in _pFootnoteList)
        {
            item.PFootnoteItemChosen = id is not null
                && item.PFootnoteItemId == id;
        }
    }

    private void PFootnoteHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PFootnoteItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PFootnoteEntryShow(item.PFootnoteItemId);
    }

    private void PFootnoteEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PFootnoteEntryHide();
            PFootnoteFind();
            return;
        }

        bool editing = PImprint.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        PImprint.PImprintDraftCancel();
        PImprint.Visibility = Visibility.Collapsed;
        PColophon.Visibility = Visibility.Collapsed;

        _pDisplayEntry = id;
        PFootnoteSelect(id);
        PDisplay.PDisplayShow(id, draft);
        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = false;

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        PFootnoteScribeShow(editing);
    }

    private void PFootnoteEntryCreate()
    {
        long? reference = _pColophonReference;

        PImprint.PImprintDraftCancel();
        PImprint.Visibility = Visibility.Collapsed;
        PColophon.Visibility = Visibility.Collapsed;

        _pDisplayEntry = null;
        PFootnoteSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = false;
        PFootnoteScribeShow(true);

        if (reference is long id)
        {
            PEditor.PEditorReferenceAdd(id);
        }
    }

    private void PFootnoteScribeHandle(bool editing)
    {
        if (_pDisplayEntry is not long id)
        {
            if (!editing)
            {
                PFootnoteScribeReset();
            }

            return;
        }

        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PReferenceLeaveConfirm())
            {
                PFootnoteScribeShow(true);
                return;
            }

            PFootnoteScribeShow(false);
            PFootnoteEntryShow(id);
            return;
        }

        PEditor.PEditorEntryShow(id);
        PFootnoteScribeShow(true);
    }

    private void PFootnoteScribeReset()
    {
        if (!PReferenceLeaveConfirm())
        {
            PFootnoteScribeShow(true);
            return;
        }

        PFootnoteScribeShow(false);

        if (_pDisplayEntry is long stored)
        {
            PFootnoteEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_pColophonReference is long kept)
        {
            PReferenceShow(kept);
            return;
        }

        PReferenceClear();
    }

    private void PFootnoteScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PReferenceViewer.IsChecked = !editing;
        PReferenceScribe.IsChecked = editing;
    }

    private void PReferenceStoreHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntrySave();
            return;
        }

        PImprint.PImprintStoreRun();
    }

    private void PFootnoteEntryUpdate(long id)
    {
        if (_pDisplayEntry is not long shown
            || (id > 0 && shown != id))
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            if (_pColophonReference is long kept)
            {
                PReferenceShow(kept);
                return;
            }

            PReferenceClear();
            return;
        }

        PDisplay.PDisplayShow(shown, draft);
    }

    private void PFootnoteEntryHide()
    {
        bool editing = PImprint.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _pDisplayEntry = null;
        PFootnoteSelect(null);
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PReferenceScribeShow(editing);
        PReferenceMode.IsEnabled = _pColophonReference is not null;
        PReferenceBin.IsEnabled = _pColophonReference is not null;
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

    private void PReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        if (_pColophonReference is not null || _pDisplayEntry is not null)
        {
            PFootnoteEntryCreate();
            return;
        }

        PReferenceClear();
        PReferenceScribeShow(true);
        PImprint.PImprintDraftOpen(null);
        PReferenceMode.IsEnabled = true;
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
