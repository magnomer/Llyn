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

    private void PAuthorApply(LReference? reference)
    {
        _pAuthorState = reference?.LReferenceAuthorState ?? LState.LStateUnspecified;
        PAuthorUnknown.IsChecked = _pAuthorState == LState.LStateUnknown;

        string? stored = PImprintReferenceRead();
        PAuthorSwitch.IsEnabled = stored is not null;
        PAuthorCreditFind(stored);
    }

    private void PAuthorCreditFind(string? stored)
    {
        _pAuthorCredit.Clear();

        if (stored is not null)
        {
            IReadOnlyList<LAuthor> credits = _pImprintOwner.PShelfCreditRead(stored);
            for (int index = 0; index < credits.Count; index++)
            {
                _pAuthorCredit.Add(new PAuthorItem(credits[index], index, credits.Count));
            }
        }

        PAuthorNotice.Visibility = _pAuthorCredit.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PAuthorNotice.SetResourceReference(
            TextBlock.TextProperty,
            stored is null ? "Source.AuthorUnsaved" : "Source.AuthorNone");
    }

    private void PAuthorUpdate()
    {
        if (!_pImprintOwner.PShelfCreditUpdate())
        {
            return;
        }

        PAuthorCreditFind(PImprintReferenceRead());
    }

    private void PAuthorHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pAuthorLoading || PImprintReferenceRead() is null || PAuthorList.SelectedValue is not long id)
        {
            return;
        }

        PAuthorSwitch.IsChecked = false;
        PAuthorList.SelectedValue = null;
        PAuthorAttach(id);
    }

    private void PAuthorAttach(long id)
    {
        if (PImprintReferenceRead() is not string stored)
        {
            return;
        }

        foreach (PAuthorItem credit in _pAuthorCredit)
        {
            if (credit.PAuthorItemId == id)
            {
                return;
            }
        }

        try
        {
            _lEngine.LEngineAuthorAttach(stored, id, _pAuthorCredit.Count);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorStateShow(LState.LStateSpecified);
        PAuthorUpdate();
    }

    private void PAuthorFreshHandle(object sender, RoutedEventArgs e)
    {
        string name = PAuthorName.Text.Trim();
        if (name.Length == 0 || PImprintReferenceRead() is null)
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
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorName.Text = string.Empty;
        PAuthorSwitch.IsChecked = false;
        PAuthorFind();
        PAuthorAttach(written.LAuthorId);
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
                PAuthorRemove(item);
                return;
        }
    }

    private void PAuthorRemove(PAuthorItem item)
    {
        if (PImprintReferenceRead() is not string stored)
        {
            return;
        }

        try
        {
            _lEngine.LEngineAuthorDetach(stored, item.PAuthorItemId);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorUpdate();
    }

    private void PAuthorMove(PAuthorItem item, int step)
    {
        if (PImprintReferenceRead() is not string stored)
        {
            return;
        }

        try
        {
            _lEngine.LEngineAuthorAttach(
                stored, item.PAuthorItemId, item.PAuthorItemPosition + step);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return;
        }

        PAuthorUpdate();
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
        PAuthorFind();
        PAuthorUpdate();
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
        if (state == LState.LStateSpecified && _pAuthorState == LState.LStateSpecified)
        {
            return;
        }

        _pAuthorState = state;
        PAuthorUnknown.IsChecked = state == LState.LStateUnknown;
        PImprintChangeDefer();
    }
}
