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
    private readonly ObservableCollection<PAuthorItem> _pAuthorCredit = [];

    private readonly ObservableCollection<PAuthorItem> _pAuthorCatalog = [];

    private LState _pAuthorState = LState.LStateUnspecified;

    private bool _pAuthorLoading;

    private void PAuthorFind()
    {
        IReadOnlyList<LAuthor> read;
        try
        {
            read = _lEngine.LEngineAuthorRead();
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.AuthorFailed", exception);
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

    private void PAuthorApply(LReference? reference)
    {
        _pAuthorState = reference?.LReferenceAuthorState ?? LState.LStateUnspecified;
        PAuthorUnknown.IsChecked = _pAuthorState == LState.LStateUnknown;
        PAuthorSwitch.IsEnabled = reference is not null;
        PAuthorCreditFind(reference);
    }

    private void PAuthorCreditFind(LReference? reference)
    {
        _pAuthorCredit.Clear();

        if (reference is not null)
        {
            IReadOnlyList<LAuthor> credits = PShelfCreditRead(reference.LReferenceId);
            for (int index = 0; index < credits.Count; index++)
            {
                _pAuthorCredit.Add(new PAuthorItem(credits[index], index, credits.Count));
            }
        }

        PAuthorNotice.Visibility = _pAuthorCredit.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PAuthorNotice.SetResourceReference(
            TextBlock.TextProperty,
            reference is null ? "Source.AuthorUnsaved" : "Source.AuthorNone");
    }

    private void PAuthorUpdate()
    {
        try
        {
            _pShelfCredit = _lEngine.LEngineAuthorRead(LOwner.LOwnerReference);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorCreditFind(_pImprintReference);
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    private void PAuthorHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pAuthorLoading || _pImprintReference is null || PAuthorList.SelectedValue is not string id)
        {
            return;
        }

        PAuthorSwitch.IsChecked = false;
        PAuthorList.SelectedValue = null;
        PAuthorAttach(id);
    }

    private void PAuthorAttach(string id)
    {
        if (_pImprintReference is null)
        {
            return;
        }

        foreach (PAuthorItem credit in _pAuthorCredit)
        {
            if (string.Equals(credit.PAuthorItemId, id, StringComparison.Ordinal))
            {
                return;
            }
        }

        try
        {
            _lEngine.LEngineAuthorAttach(_pImprintReference.LReferenceId, id, _pAuthorCredit.Count);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorStateShow(LState.LStateSpecified);
        PAuthorUpdate();
    }

    private void PAuthorFreshHandle(object sender, RoutedEventArgs e)
    {
        string name = PAuthorName.Text.Trim();
        if (name.Length == 0 || _pImprintReference is null)
        {
            return;
        }

        LAuthor written;
        try
        {
            written = _lEngine.LEngineAuthorCreate(new LAuthor(string.Empty, name));
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorName.Text = string.Empty;
        PAuthorSwitch.IsChecked = false;
        PAuthorFind();
        PAuthorAttach(written.LAuthorId);
    }

    private void PAuthorRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PAuthorItem item } || _pImprintReference is null)
        {
            return;
        }

        try
        {
            _lEngine.LEngineAuthorDetach(_pImprintReference.LReferenceId, item.PAuthorItemId);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorUpdate();
    }

    private void PAuthorEarlierHandle(object sender, RoutedEventArgs e)
    {
        PAuthorMove(sender, -1);
    }

    private void PAuthorLaterHandle(object sender, RoutedEventArgs e)
    {
        PAuthorMove(sender, 1);
    }

    private void PAuthorMove(object sender, int step)
    {
        if (sender is not FrameworkElement { DataContext: PAuthorItem item } || _pImprintReference is null)
        {
            return;
        }

        try
        {
            _lEngine.LEngineAuthorAttach(
                _pImprintReference.LReferenceId, item.PAuthorItemId, item.PAuthorItemPosition + step);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorUpdate();
    }

    private void PAuthorRenameHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PAuthorItem item })
        {
            return;
        }

        string name = PAuthorName.Text.Trim();
        if (name.Length == 0)
        {
            PAuthorSwitch.IsChecked = true;
            PAuthorName.Focus();
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
            _pReferenceHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorName.Text = string.Empty;
        PAuthorFind();
        PAuthorUpdate();
    }

    private bool PAuthorRenameConfirm(PAuthorItem item)
    {
        int reach = 0;
        foreach (IReadOnlyList<LAuthor> credits in _pShelfCredit.Values)
        {
            foreach (LAuthor author in credits)
            {
                if (string.Equals(author.LAuthorId, item.PAuthorItemId, StringComparison.Ordinal))
                {
                    reach++;
                    break;
                }
            }
        }

        string count = $"{_pReferenceHost.PLocalizationTextRead("Source.AuthorRenameCount")} "
            + reach.ToString(CultureInfo.CurrentCulture);

        return MessageBox.Show(
            _pReferenceHost,
            $"{_pReferenceHost.PLocalizationTextRead("Source.AuthorRenameConfirm")}\n\n{count}",
            _pReferenceHost.PLocalizationTextRead("Terms.Product"),
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
        if (state == LState.LStateSpecified && _pAuthorState == LState.LStateSpecified)
        {
            return;
        }

        _pAuthorState = state;
        PAuthorUnknown.IsChecked = state == LState.LStateUnknown;
    }
}
