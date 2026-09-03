using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const string PLinkFailureKey = "Input.TranslationFailed";

    private readonly ObservableCollection<PLinkItem> _pLinkItem = [];

    private readonly List<string> _pLinkFresh = [];

    private PCard? _pLinkCard;

    internal void PLinkAttach(PCard card)
    {
        card.PCardLinkDispatcher = (text, offered) => PLinkResolve(card, text, offered);
    }

    internal void PLinkChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLinkChip chip })
        {
            PCardLinkFind(chip)?.PCardLinkRemove(chip);
        }
    }

    internal void PLinkCaretHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLinkCaret row } box)
        {
            return;
        }

        PCard? card = PCardLinkFind(row);
        if (card is null)
        {
            return;
        }

        if (PLinkMenuChoice.IsOpen && PLinkMenuHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            card.PCardLinkCommit();
            e.Handled = true;
            return;
        }

        if (box.SelectionLength != 0)
        {
            return;
        }

        if (e.Key == Key.Back && box.CaretIndex == 0)
        {
            card.PCardLinkRemove(-1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            card.PCardLinkRemove(1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Left && box.Text.Length == 0 && card.PCardLinkMove(-1))
        {
            PLinkCaretApply(box, row, 0);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && box.Text.Length == 0 && card.PCardLinkMove(1))
        {
            PLinkCaretApply(box, row, 0);
            e.Handled = true;
        }
    }

    internal void PLinkCloseHandle(object sender, RoutedEventArgs e)
    {
        if (PLinkMenuChoice.IsOpen ||
            sender is not FrameworkElement { DataContext: PLinkCaret row })
        {
            return;
        }

        PCard? card = PCardLinkFind(row);
        if (card is not null && PLinkResolve(card, row.PLinkCaretText, false))
        {
            card.PCardLinkClear();
        }
    }

    internal void PLinkFocusHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = PLinkCaretFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    internal void PLinkMenuHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLinkItem item })
        {
            PLinkMenuHide();
            return;
        }

        PLinkMenuSelect(item);
        e.Handled = true;
    }

    internal void PLinkFlagUpdate()
    {
        foreach (PCard card in _pSenseList)
        {
            card.PCardFlagUpdate();
        }

        foreach (PCard card in _pCollocationList)
        {
            card.PCardFlagUpdate();
        }
    }

    internal void PLinkFreshClear()
    {
        _pLinkFresh.Clear();
    }

    internal void PLinkFreshDelete()
    {
        foreach (string id in _pLinkFresh)
        {
            try
            {
                _lEngine.LEngineTranslationDelete(id);
            }
            catch (Exception)
            {
                continue;
            }
        }

        _pLinkFresh.Clear();
    }

    private bool PLinkMenuHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PLinkMenuHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pLinkItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PLinkMenuList.SelectedIndex < 0 ? 0 : PLinkMenuList.SelectedIndex;
            PLinkMenuList.SelectedIndex = (chosen + step) % count;
            PLinkMenuList.ScrollIntoView(PLinkMenuList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PLinkMenuList.SelectedItem is PLinkItem item)
        {
            PLinkMenuSelect(item);
            return true;
        }

        return false;
    }

    private void PLinkMenuSelect(PLinkItem item)
    {
        PCard? card = _pLinkCard;
        if (card is null)
        {
            PLinkMenuHide();
            return;
        }

        if (!item.PLinkItemFresh)
        {
            card.PCardLinkCommit(
                item.PLinkItemId, item.PLinkItemHeadword, item.PLinkItemLanguage);
            card.PCardLinkClear();
            PLinkMenuHide();
            return;
        }

        LEntry created;
        try
        {
            created = _lEngine.LEngineTranslationCreate(
                item.PLinkItemHeadword, item.PLinkItemLanguage);
        }
        catch (Exception exception)
        {
            PLinkMenuHide();
            _pEditorHost.PWindowFailureShow(PLinkFailureKey, exception);
            return;
        }

        _pLinkFresh.Add(created.LEntryId);
        card.PCardLinkCommit(created.LEntryId, created.LEntryHeadword, created.LEntryLanguage);
        card.PCardLinkClear();
        PLinkMenuHide();
    }

    private bool PLinkResolve(PCard card, string text, bool offered)
    {
        string word = (text ?? string.Empty).Trim();
        if (word.Length == 0)
        {
            return false;
        }

        LEntry? single;
        IReadOnlyList<LEntry> found;
        try
        {
            single = _lEngine.LEngineTranslationResolve(word, _pEditorEntry);
            found = single is null ? _lEngine.LEngineTranslationFind(word, _pEditorEntry) : [];
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow(PLinkFailureKey, exception);
            return false;
        }

        if (single is not null)
        {
            return card.PCardLinkCommit(
                single.LEntryId, single.LEntryHeadword, single.LEntryLanguage);
        }

        if (offered)
        {
            PLinkMenuShow(card, word, found);
        }

        return false;
    }

    private void PLinkMenuShow(PCard card, string word, IReadOnlyList<LEntry> found)
    {
        _pLinkItem.Clear();
        foreach (LEntry entry in found)
        {
            _pLinkItem.Add(new PLinkItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage, false));
        }

        foreach (string language in PLinkLanguageRead())
        {
            _pLinkItem.Add(new PLinkItem(string.Empty, word, language, true));
        }

        _pLinkCard = card;
        PLinkMenuChoice.PlacementTarget = PLinkBoxFind(card) ?? (UIElement)PContents;
        PLinkMenuChoice.IsOpen = true;
        PLinkMenuList.SelectedIndex = 0;
    }

    private void PLinkMenuHide()
    {
        PLinkMenuChoice.IsOpen = false;
        PLinkMenuList.SelectedIndex = -1;
        _pLinkItem.Clear();
        _pLinkCard = null;
    }

    private IReadOnlyList<string> PLinkLanguageRead()
    {
        List<string> languages = [];
        foreach (PTongueItem item in _pTongueItem)
        {
            if (!string.Equals(item.PTongueItemName, _pLanguageChoice, StringComparison.Ordinal))
            {
                languages.Add(item.PTongueItemName);
            }
        }

        languages.Add(_pLanguageChoice);
        return languages;
    }

    private TextBox? PLinkBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PLinkCaret row } box &&
            PCardLinkFind(row) == card
            ? box
            : null;
    }

    private static void PLinkCaretApply(TextBox box, PLinkCaret row, int caret)
    {
        ItemsControl? host = ItemsControl.ItemsControlFromItemContainer(box) ?? PLinkHostFind(box);
        box.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                TextBox? entry = host is null ? box : PLinkCaretFind(host) ?? box;
                if (entry.DataContext != row)
                {
                    return;
                }

                entry.Focus();
                entry.CaretIndex = caret > entry.Text.Length ? entry.Text.Length : caret;
            });
    }

    private static ItemsControl? PLinkHostFind(DependencyObject start)
    {
        DependencyObject? step = start;
        while (step is not null)
        {
            if (step is ItemsControl host)
            {
                return host;
            }

            step = VisualTreeHelper.GetParent(step);
        }

        return null;
    }

    private static TextBox? PLinkCaretFind(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            if (child is TextBox { DataContext: PLinkCaret } box)
            {
                return box;
            }

            TextBox? found = PLinkCaretFind(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private PCard? PCardLinkFind(object row)
    {
        foreach (PCard card in _pSenseList)
        {
            if (card.PCardLink.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardLink.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
