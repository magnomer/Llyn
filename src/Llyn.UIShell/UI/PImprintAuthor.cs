using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PImprint
{
    private readonly ObservableCollection<PAuthorItem> _pAuthorCredit = [];

    private IReadOnlyList<LAuthor> _pAuthorCatalog = [];

    private LState _pAuthorState = LState.LStateUnspecified;

    private IReadOnlyList<LAuthor> _pAuthorCredited = [];

    private int _pAuthorBlankAt = -1;

    internal void PAuthorFind()
    {
        try
        {
            _pAuthorCatalog = _lEngine.LEngineAuthorRead();
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            _pAuthorCatalog = [];
        }
    }

    private void PAuthorOpen(LDraft? draft)
    {
        _pAuthorBlankAt = -1;
        PAuthorShow(draft);
    }

    private void PAuthorShow(LDraft? draft)
    {
        _pAuthorState = draft?.LDraftReference?.LReferenceAuthorState.LStateMarkState ?? LState.LStateUnspecified;
        PAuthorNotice.Visibility = draft is null ? Visibility.Visible : Visibility.Collapsed;
        PAuthorCreditShow(draft is null ? [] : PAuthorCreditRead(draft));
    }

    private void PAuthorCreditShow(IReadOnlyList<LAuthor> credits)
    {
        _pAuthorCredited = credits;

        if (credits.Count == 0 && _pAuthorBlankAt < 0 && _pImprintDraft != 0)
        {
            _pAuthorBlankAt = 0;
        }

        if (_pAuthorBlankAt > credits.Count)
        {
            _pAuthorBlankAt = credits.Count;
        }

        if (PAuthorCreditMatch(credits))
        {
            return;
        }

        PBylineHide();
        _pAuthorCredit.Clear();
        for (int index = 0; index < credits.Count; index++)
        {
            if (index == _pAuthorBlankAt)
            {
                _pAuthorCredit.Add(new PAuthorItem(index));
            }

            _pAuthorCredit.Add(new PAuthorItem(credits[index], index, credits.Count));
        }

        if (_pAuthorBlankAt == credits.Count)
        {
            _pAuthorCredit.Add(new PAuthorItem(credits.Count));
        }
    }

    private IReadOnlyList<LAuthor> PAuthorCreditRead(LDraft draft)
    {
        List<LAuthor> credits = new(draft.LDraftAuthor.Count);
        foreach (LAuthor author in draft.LDraftAuthor)
        {
            credits.Add(PAuthorCatalogFind(author.LAuthorId) is LAuthor named
                ? author with { LAuthorName = named.LAuthorName }
                : author);
        }

        return credits;
    }

    private LAuthor? PAuthorCatalogFind(long id)
    {
        foreach (LAuthor author in _pAuthorCatalog)
        {
            if (author.LAuthorId == id)
            {
                return author;
            }
        }

        return null;
    }

    private PAuthorItem? PAuthorCreditFind(long id)
    {
        foreach (PAuthorItem credit in _pAuthorCredit)
        {
            if (credit.PAuthorItemId == id)
            {
                return credit;
            }
        }

        return null;
    }

    private bool PAuthorCreditMatch(IReadOnlyList<LAuthor> credits)
    {
        if (_pAuthorCredit.Count != credits.Count + (_pAuthorBlankAt < 0 ? 0 : 1))
        {
            return false;
        }

        int shown = 0;
        foreach (PAuthorItem row in _pAuthorCredit)
        {
            if (row.PAuthorItemBlank)
            {
                if (row.PAuthorItemPosition != _pAuthorBlankAt)
                {
                    return false;
                }

                continue;
            }

            if (row.PAuthorItemId != credits[shown].LAuthorId
                || !string.Equals(row.PAuthorItemName, credits[shown].LAuthorName, StringComparison.Ordinal))
            {
                return false;
            }

            shown++;
        }

        return true;
    }

    private void PAuthorAdd(PAuthorItem item)
    {
        if (_pImprintDraft == 0)
        {
            return;
        }

        if (!item.PAuthorItemBlank)
        {
            _pAuthorBlankAt = item.PAuthorItemPosition + 1;
            PAuthorCreditShow(_pAuthorCredited);
        }

        foreach (PAuthorItem row in _pAuthorCredit)
        {
            if (row.PAuthorItemBlank)
            {
                PAuthorSelect(row);
                return;
            }
        }
    }

    private void PAuthorSelect(PAuthorItem item)
    {
        Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            () =>
            {
                if (PAuthorCredit.ItemContainerGenerator.ContainerFromItem(item) is DependencyObject container
                    && PEditor.PEditorCaretFind(container) is TextBox box)
                {
                    box.Focus();
                    box.CaretIndex = box.Text.Length;
                }
            });
    }

    private void PAuthorCreditHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement { Tag: string action, DataContext: PAuthorItem item })
        {
            return;
        }

        switch (action)
        {
            case "Add":
                PAuthorAdd(item);
                return;
            case "Earlier":
                PAuthorMove(item, -1);
                return;
            case "Later":
                PAuthorMove(item, 1);
                return;
            default:
                PAuthorRemove(item);
                return;
        }
    }

    private void PAuthorMove(PAuthorItem item, int step)
    {
        if (item.PAuthorItemBlank)
        {
            return;
        }

        PAuthorRequestSend(
            new LRequestAuthorShift(_pImprintDraft, item.PAuthorItemId, item.PAuthorItemPosition + step));
    }

    private void PAuthorRemove(PAuthorItem item)
    {
        PBylineHide();

        if (item.PAuthorItemBlank)
        {
            _pAuthorBlankAt = -1;
            PAuthorCreditShow(_pAuthorCredited);
            return;
        }

        PAuthorRequestSend(new LRequestAuthorRemoval(_pImprintDraft, item.PAuthorItemId));
    }

    private void PAuthorTextHandle(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is not TextBox { IsKeyboardFocusWithin: true } box || _pImprintDraft == 0)
        {
            return;
        }

        PBylineShow(box, box.Text);
    }

    private void PAuthorKeyHandle(object sender, KeyEventArgs e)
    {
        if (e.OriginalSource is not TextBox { DataContext: PAuthorItem row } box)
        {
            return;
        }

        if (PByline.IsOpen && PBylineHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            PAuthorCommit(row, box);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            box.Text = row.PAuthorItemName;
            PBylineHide();
            e.Handled = true;
        }
    }

    private void PAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.OriginalSource is not TextBox { DataContext: PAuthorItem row } box)
        {
            return;
        }

        if (_pBylineBox == box)
        {
            PBylineHide();
        }

        if (!string.Equals(box.Text, row.PAuthorItemName, StringComparison.Ordinal))
        {
            box.Text = row.PAuthorItemName;
        }
    }

    private void PAuthorCommit(PAuthorItem row, TextBox box)
    {
        PBylineHide();

        string name = box.Text.Trim();
        if (name.Length == 0)
        {
            box.Text = row.PAuthorItemName;
            return;
        }

        if (!row.PAuthorItemBlank && string.Equals(name, row.PAuthorItemName, StringComparison.Ordinal))
        {
            return;
        }

        PAuthorChange(row, new LRequestAuthorAddition(_pImprintDraft, name, row.PAuthorItemPosition));
    }

    private void PAuthorAttach(PAuthorItem row, TextBox box, long id)
    {
        if (id == row.PAuthorItemId)
        {
            box.Text = row.PAuthorItemName;
            return;
        }

        PAuthorChange(row, new LRequestAuthorPick(_pImprintDraft, id, row.PAuthorItemPosition));
    }

    private void PAuthorChange(PAuthorItem row, LRequest request)
    {
        if (_pImprintDraft == 0)
        {
            return;
        }

        int blank = _pAuthorBlankAt;
        _pAuthorBlankAt = -1;

        if (!PAuthorRequestSend(request))
        {
            _pAuthorBlankAt = blank;
            return;
        }

        if (!row.PAuthorItemBlank)
        {
            PAuthorRequestSend(new LRequestAuthorRemoval(_pImprintDraft, row.PAuthorItemId));
        }
    }

    private bool PAuthorRequestSend(LRequest request)
    {
        if (_pImprintDraft == 0 || _pImprintHalted)
        {
            return false;
        }

        PImprintChangeSave();

        try
        {
            _lEngine.LEngineRequestApply(request);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.AuthorFailed", exception);
            return false;
        }

        PImprintChangeUpdate();
        return true;
    }
}
