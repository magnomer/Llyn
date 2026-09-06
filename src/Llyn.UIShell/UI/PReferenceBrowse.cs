using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PReference
{
    private static readonly IReadOnlyList<LAuthor> PShelfNobody = [];

    private readonly ObservableCollection<PShelfItem> _pShelfList = [];

    private readonly ObservableCollection<PFootnoteItem> _pFootnoteList = [];

    private IReadOnlyDictionary<string, int> _pShelfCount = new Dictionary<string, int>();

    private IReadOnlyDictionary<string, IReadOnlyList<LAuthor>> _pShelfCredit =
        new Dictionary<string, IReadOnlyList<LAuthor>>();

    private string? _pColophonReference;

    private LCatalogOrder _pGradeChoice = LCatalogOrder.LCatalogOrderName;

    private void PReferenceHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PShelf.ItemsSource = _pShelfList;
        PFootnote.ItemsSource = _pFootnoteList;
        PAuthorCredit.ItemsSource = _pAuthorCredit;
        PAuthorList.ItemsSource = _pAuthorCatalog;

        PAuthorFind();
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

        _pGradeChoice = LCatalog.LCatalogOrderParse(choice, LCatalogOrder.LCatalogOrderName);
        PGradeDropper.IsChecked = false;
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    private IReadOnlyList<LAuthor> PShelfCreditRead(string id)
    {
        return _pShelfCredit.TryGetValue(id, out IReadOnlyList<LAuthor>? credits) ? credits : PShelfNobody;
    }

    private int PShelfCountRead(string id)
    {
        return _pShelfCount.TryGetValue(id, out int usage) ? usage : 0;
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
            kept |= string.Equals(
                row.LCatalogReferenceStored.LReferenceId,
                _pColophonReference,
                StringComparison.Ordinal);
            _pShelfList.Add(new PShelfItem(row, unreadable, unset));
        }

        PShelfEmpty.Visibility = _pShelfList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept && _pColophonReference is not null && PImprint.Visibility != Visibility.Visible)
        {
            PReferenceClear();
        }
    }

    private void PShelfHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PShelfItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PReferenceShow(item.PShelfItemId);
    }

    private void PReferenceShow(string id)
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

        PColophonValueShow(PColophonTitle, reference.LReferenceTitle);
        PColophonValueShow(PColophonProgram, reference.LReferenceProgram);
        PColophonValueShow(PColophonChannel, reference.LReferenceChannel);
        PColophonValueShow(PColophonYear, reference.LReferenceYear);
        PColophonValueShow(PColophonUrl, reference.LReferenceUrl);
        PColophonAuthorShow(reference);

        PFootnoteFind(id);

        PColophonBody.Visibility = Visibility.Visible;
        PColophonUnselected.Visibility = Visibility.Collapsed;
        PReferenceScribe.IsEnabled = true;

        if (PImprint.Visibility == Visibility.Visible)
        {
            PImprintDraftShow(PImprintDraftStart(id));
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

    private void PFootnoteFind(string id)
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
        string entry = _pReferenceHost.PLocalizationTextRead("Source.Entry");
        string example = _pReferenceHost.PLocalizationTextRead("Source.Example");

        _pFootnoteList.Clear();
        foreach (LUsage usage in read)
        {
            _pFootnoteList.Add(new PFootnoteItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerEntry ? entry : example,
                unreadable,
                unnamed));
        }

        PFootnoteEmpty.Visibility = _pFootnoteList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PFootnoteHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PFootnoteItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        if (item.PFootnoteItemOwner == LOwner.LOwnerEntry)
        {
            _pReferenceHost.PWindowEntryShow(item.PFootnoteItemId);
            return;
        }

        _pReferenceHost.PWindowExampleShow(item.PFootnoteItemId);
    }

    private void PReferenceScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PImprint.Visibility == Visibility.Visible)
        {
            if (!PReferenceLeaveConfirm())
            {
                return;
            }

            PImprintDraftCancel();
            PReferenceScribeShow(false);

            if (_pColophonReference is not null)
            {
                PReferenceShow(_pColophonReference);
                return;
            }

            PReferenceClear();
            return;
        }

        if (_pColophonReference is null)
        {
            return;
        }

        PReferenceScribeShow(true);
        PImprintDraftShow(PImprintDraftStart(_pColophonReference));
    }

    private void PReferenceScribeShow(bool editing)
    {
        PImprint.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PColophon.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PReferenceScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    internal bool PReferenceLeaveConfirm()
    {
        return _pReferenceHost.PWindowDiscardConfirm(PReferenceChangeCheck());
    }

    private void PReferenceClear()
    {
        PImprintDraftCancel();

        _pColophonReference = null;
        _pFootnoteList.Clear();

        PColophonBody.Visibility = Visibility.Collapsed;
        PColophonUnselected.Visibility = Visibility.Visible;
        PImprintApply(null);
        PReferenceScribeShow(false);
        PReferenceScribe.IsEnabled = false;
    }
}
