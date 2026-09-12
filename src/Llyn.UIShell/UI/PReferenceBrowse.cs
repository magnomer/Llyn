using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    private LCatalogOrder _pGradeChoice;

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

        PImprint.PImprintBulletinHandle(bulletin);
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    private void PSurveyHandle(object sender, TextChangedEventArgs e)
    {
        PShelfFind(PSurvey.Text ?? string.Empty);
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

    internal int PShelfReachRead(long author)
    {
        int reach = 0;
        foreach (IReadOnlyList<LAuthor> credits in _pShelfCredit.Values)
        {
            foreach (LAuthor written in credits)
            {
                if (written.LAuthorId == author)
                {
                    reach++;
                    break;
                }
            }
        }

        return reach;
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

        string unreadable = _pReferenceHost.PLocalizationTextRead("Display.Unreadable");
        string unset = _pReferenceHost.PLocalizationTextRead("Source.Unset");

        _pShelfList.Clear();
        bool kept = false;
        foreach (LCatalogReference row in read)
        {
            kept |= row.LCatalogReferenceStored.LReferenceId == _pColophonReference;
            _pShelfList.Add(new PShelfItem(row, unreadable, unset));
        }

        PShelfEmpty.Visibility = _pShelfList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PShelfSelect(_pColophonReference);

        if (!kept && _pColophonReference is not null && PImprint.Visibility != Visibility.Visible)
        {
            PReferenceClear();
        }
    }

    private void PShelfHandle(object sender, RoutedEventArgs e)
    {
        if (e.Source is not FrameworkElement row || row.DataContext is not PShelfItem item)
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

        PColophonValueShow(PColophonTitle, reference.LReferenceTitle);
        PColophonValueShow(PColophonYear, reference.LReferenceYear);
        PColophonKind.Text = PReferenceKindShow(reference.LReferenceKind);
        PColophonValueShow(PColophonNote, reference.LReferenceNote);
        PColophonValueShow(PColophonUrl, reference.LReferenceUrl);
        PColophonAuthorShow(reference);

        PFootnoteFind(id);

        PColophonBody.Visibility = Visibility.Visible;
        PColophonUnselected.Visibility = Visibility.Collapsed;
        PReferenceMode.IsEnabled = true;

        if (PImprint.Visibility == Visibility.Visible)
        {
            PImprint.PImprintDraftOpen(id);
        }
    }

    private void PColophonValueShow(TextBlock field, LStateValue value)
    {
        string? text = value.LStateValueState switch
        {
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => _pReferenceHost.PLocalizationTextRead("Display.Unreadable"),
            _ => null,
        };

        field.Text = text ?? _pReferenceHost.PLocalizationTextRead("Source.Unset");
        field.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void PColophonAuthorShow(LReference reference)
    {
        IReadOnlyList<LAuthor> credits = PShelfCreditRead(reference.LReferenceId);
        string text = PShelfItem.PShelfCreditRead(
            reference,
            credits,
            _pReferenceHost.PLocalizationTextRead("Display.Unreadable"),
            _pReferenceHost.PLocalizationTextRead("Source.Unset"));

        PColophonAuthor.Text = text;
        PColophonAuthor.SetResourceReference(
            TextBlock.ForegroundProperty,
            credits.Count == 0 ? "Theme.Muted" : "Theme.Ink");
    }

    private void PFootnoteFind(long id)
    {
        IReadOnlyList<LUsage> read;
        try
        {
            read = _lEngine.LEngineUsageRead(id, LOwner.LOwnerReference);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        string unreadable = _pReferenceHost.PLocalizationTextRead("Display.Unreadable");
        string unnamed = _pReferenceHost.PLocalizationTextRead("Source.Unnamed");
        string meaning = _pReferenceHost.PLocalizationTextRead("Source.Meaning");
        string collocation = _pReferenceHost.PLocalizationTextRead("Source.Collocation");
        string example = _pReferenceHost.PLocalizationTextRead("Source.Example");

        _pFootnoteList.Clear();
        foreach (LUsage usage in read)
        {
            _pFootnoteList.Add(new PFootnoteItem(
                usage,
                usage.LUsageOwner switch
                {
                    LOwner.LOwnerMeaning => meaning,
                    LOwner.LOwnerCollocation => collocation,
                    _ => example,
                },
                unreadable,
                unnamed));
        }

        PFootnoteEmpty.Visibility = _pFootnoteList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PFootnoteHandle(object sender, RoutedEventArgs e)
    {
        if (e.Source is not FrameworkElement row || row.DataContext is not PFootnoteItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        if (item.PFootnoteItemOwner != LOwner.LOwnerExample)
        {
            _pReferenceHost.PWindowEntryShow(item.PFootnoteItemId);
            return;
        }

        _pReferenceHost.PWindowExampleShow(item.PFootnoteItemId);
    }

    private void PReferenceScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PReferenceScribe);
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
        return _pReferenceHost.PWindowDiscardConfirm(PReferenceChangeCheck());
    }

    private void PReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PReferenceClear();
        PReferenceScribeShow(true);
        PImprint.PImprintDraftOpen(null);
        PReferenceScribe.IsEnabled = true;
    }

    internal void PReferenceClear()
    {
        PImprint.PImprintDraftCancel();

        _pColophonReference = null;
        PShelfSelect(null);
        _pFootnoteList.Clear();

        PColophonBody.Visibility = Visibility.Collapsed;
        PColophonUnselected.Visibility = Visibility.Visible;
        PImprint.PImprintClear();
        PReferenceScribeShow(false);
        PReferenceMode.IsEnabled = false;
    }
}
