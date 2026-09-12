using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PImprint
{
    private readonly ObservableCollection<PAuthorItem> _pAuthorCredit = [];

    private readonly ObservableCollection<PAuthorItem> _pAuthorCatalog = [];

    private LState _pAuthorState = LState.LStateUnspecified;

    private bool _pAuthorLoading;

    internal void PAuthorFind()
    {
        IReadOnlyList<LAuthor> read;
        try
        {
            read = _lEngine.LEngineAuthorRead();
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            read = [];
        }

        _pAuthorLoading = true;
        _pAuthorCatalog.Clear();
        for (int index = 0; index < read.Count; index++)
        {
            _pAuthorCatalog.Add(new PAuthorItem(read[index], index, read.Count));
        }

        PAuthorList.SelectedValue = null;
        _pAuthorLoading = false;

        PAuthorEmpty.Visibility = _pAuthorCatalog.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PAuthorShow(LDraft? draft)
    {
        _pAuthorState = draft?.LDraftReference?.LReferenceAuthorState.LStateMarkState ?? LState.LStateUnspecified;
        PAuthorUnknown.IsChecked = _pAuthorState == LState.LStateUnknown;
        PAuthorSwitch.IsEnabled = draft is not null;
        PAuthorCreditShow(draft is null ? [] : PAuthorCreditRead(draft));

        PAuthorNotice.Visibility = _pAuthorCredit.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PAuthorNotice.SetResourceReference(
            TextBlock.TextProperty,
            draft is null ? "Source.AuthorUnsaved" : "Source.AuthorNone");
    }

    private void PAuthorCreditShow(IReadOnlyList<LAuthor> credits)
    {
        if (PAuthorCreditMatch(credits))
        {
            return;
        }

        _pAuthorCredit.Clear();
        for (int index = 0; index < credits.Count; index++)
        {
            _pAuthorCredit.Add(new PAuthorItem(credits[index], index, credits.Count));
        }
    }

    private IReadOnlyList<LAuthor> PAuthorCreditRead(LDraft draft)
    {
        List<LAuthor> credits = new(draft.LDraftAuthor.Count);
        foreach (LAuthor author in draft.LDraftAuthor)
        {
            credits.Add(PAuthorCatalogFind(author.LAuthorId) is PAuthorItem named
                ? author with { LAuthorName = named.PAuthorItemName }
                : author);
        }

        return credits;
    }

    private PAuthorItem? PAuthorCatalogFind(long id)
    {
        foreach (PAuthorItem item in _pAuthorCatalog)
        {
            if (item.PAuthorItemId == id)
            {
                return item;
            }
        }

        return null;
    }

    private bool PAuthorCreditMatch(IReadOnlyList<LAuthor> credits)
    {
        if (_pAuthorCredit.Count != credits.Count)
        {
            return false;
        }

        for (int index = 0; index < credits.Count; index++)
        {
            if (_pAuthorCredit[index].PAuthorItemId != credits[index].LAuthorId
                || !string.Equals(
                    _pAuthorCredit[index].PAuthorItemName, credits[index].LAuthorName, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private void PAuthorHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pAuthorLoading || _pImprintDraft == 0 || PAuthorList.SelectedValue is not long id)
        {
            return;
        }

        PAuthorSwitch.IsChecked = false;
        PAuthorList.SelectedValue = null;
        PAuthorAttach(id);
    }

    private void PAuthorAttach(long id)
    {
        foreach (PAuthorItem credit in _pAuthorCredit)
        {
            if (credit.PAuthorItemId == id)
            {
                return;
            }
        }

        PAuthorRequestSend(new LRequestAuthorPick(_pImprintDraft, id, _pAuthorCredit.Count));
    }

    private void PAuthorFreshHandle(object sender, RoutedEventArgs e)
    {
        string name = PAuthorName.Text.Trim();
        if (name.Length == 0 || _pImprintDraft == 0)
        {
            return;
        }

        PAuthorName.Text = string.Empty;
        PAuthorSwitch.IsChecked = false;
        PAuthorRequestSend(new LRequestAuthorAddition(_pImprintDraft, name, _pAuthorCredit.Count));
    }

    private void PAuthorCreditHandle(object sender, RoutedEventArgs e)
    {
        if (e.Source is not FrameworkElement { Tag: string action, DataContext: PAuthorItem item })
        {
            return;
        }

        switch (action)
        {
            case "Earlier":
                PAuthorMove(item, -1);
                return;
            case "Later":
                PAuthorMove(item, 1);
                return;
            case "Rename":
                PAuthorNameUpdate(item);
                return;
            default:
                PAuthorRequestSend(new LRequestAuthorRemoval(_pImprintDraft, item.PAuthorItemId));
                return;
        }
    }

    private void PAuthorMove(PAuthorItem item, int step)
    {
        PAuthorRequestSend(
            new LRequestAuthorShift(_pImprintDraft, item.PAuthorItemId, item.PAuthorItemPosition + step));
    }

    private void PAuthorNameUpdate(PAuthorItem item)
    {
        string name = PAuthorName.Text.Trim();
        if (name.Length == 0)
        {
            PAuthorSwitch.IsChecked = true;
            PAuthorName.Focus();
            return;
        }

        if (item.PAuthorItemId < 0)
        {
            PAuthorName.Text = string.Empty;
            PAuthorRequestSend(new LRequestAuthorRemoval(_pImprintDraft, item.PAuthorItemId));
            PAuthorRequestSend(new LRequestAuthorAddition(_pImprintDraft, name, item.PAuthorItemPosition));
            return;
        }

        if (!PAuthorRenameConfirm(item))
        {
            return;
        }

        try
        {
            _lEngine.LEngineAuthorUpdate(new LAuthor(item.PAuthorItemId, name));
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorName.Text = string.Empty;
    }

    private void PAuthorRequestSend(LRequest request)
    {
        if (_pImprintDraft == 0 || _pImprintHalted)
        {
            return;
        }

        PImprintChangeSave();

        try
        {
            _lEngine.LEngineRequestApply(request);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PImprintChangeUpdate();
    }

    private bool PAuthorRenameConfirm(PAuthorItem item)
    {
        int reach = _pImprintOwner.PShelfReachRead(item.PAuthorItemId);

        string count = $"{_pImprintHost.PLocalizationTextRead("Source.AuthorRenameCount")} "
            + reach.ToString(CultureInfo.CurrentCulture);

        return MessageBox.Show(
            _pImprintHost,
            $"{_pImprintHost.PLocalizationTextRead("Source.AuthorRenameConfirm")}\n\n{count}",
            _pImprintHost.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    private void PAuthorUnknownHandle(object sender, RoutedEventArgs e)
    {
        PAuthorStateShow(PAuthorUnknown.IsChecked == true
            ? LState.LStateUnknown
            : LState.LStateUnspecified);
    }

    private void PAuthorStateShow(LState state)
    {
        if (_pAuthorState == state)
        {
            return;
        }

        _pAuthorState = state;
        PAuthorUnknown.IsChecked = state == LState.LStateUnknown;
        PImprintChangeDefer();
    }
}
