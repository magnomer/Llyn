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
    private const string PTranslationFailureKey = "Input.TranslationFailed";

    private readonly ObservableCollection<PTranslationItem> _pTranslationItem = [];

    private readonly List<string> _pTranslationFresh = [];

    private PCard? _pTranslationCard;

    internal void PTranslationAttach(PCard card)
    {
        card.PCardTranslationDispatcher = (text, offered) => PTranslationResolve(card, text, offered);
    }

    internal void PTranslationChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PTranslationChip chip })
        {
            PCardTranslationFind(chip)?.PCardTranslationRemove(chip);
        }
    }

    internal void PTranslationEntryHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PTranslationEntry row } box)
        {
            return;
        }

        PCard? card = PCardTranslationFind(row);
        if (card is null)
        {
            return;
        }

        if (PTranslationMenuChoice.IsOpen && PTranslationMenuHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            card.PCardTranslationCommit();
            e.Handled = true;
            return;
        }

        if (box.SelectionLength != 0)
        {
            return;
        }

        if (e.Key == Key.Back && box.CaretIndex == 0)
        {
            card.PCardTranslationRemove(-1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            card.PCardTranslationRemove(1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Left && box.Text.Length == 0 && card.PCardTranslationMove(-1))
        {
            PTranslationEntryApply(box, row, 0);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && box.Text.Length == 0 && card.PCardTranslationMove(1))
        {
            PTranslationEntryApply(box, row, 0);
            e.Handled = true;
        }
    }

    internal void PTranslationCloseHandle(object sender, RoutedEventArgs e)
    {
        if (PTranslationMenuChoice.IsOpen ||
            sender is not FrameworkElement { DataContext: PTranslationEntry row })
        {
            return;
        }

        PCard? card = PCardTranslationFind(row);
        if (card is not null && PTranslationResolve(card, row.PTranslationEntryText, false))
        {
            card.PCardTranslationClear();
        }
    }

    internal void PTranslationFocusHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = PTranslationEntryFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    internal void PTranslationMenuHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PTranslationItem item })
        {
            PTranslationMenuHide();
            return;
        }

        PTranslationMenuSelect(item);
        e.Handled = true;
    }

    internal void PTranslationFlagUpdate()
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

    internal void PTranslationFreshClear()
    {
        _pTranslationFresh.Clear();
    }

    internal void PTranslationFreshDelete()
    {
        foreach (string id in _pTranslationFresh)
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

        _pTranslationFresh.Clear();
    }

    private bool PTranslationMenuHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PTranslationMenuHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pTranslationItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PTranslationMenuList.SelectedIndex < 0 ? 0 : PTranslationMenuList.SelectedIndex;
            PTranslationMenuList.SelectedIndex = (chosen + step) % count;
            PTranslationMenuList.ScrollIntoView(PTranslationMenuList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PTranslationMenuList.SelectedItem is PTranslationItem item)
        {
            PTranslationMenuSelect(item);
            return true;
        }

        return false;
    }

    private void PTranslationMenuSelect(PTranslationItem item)
    {
        PCard? card = _pTranslationCard;
        if (card is null)
        {
            PTranslationMenuHide();
            return;
        }

        if (!item.PTranslationItemFresh)
        {
            card.PCardTranslationCommit(
                item.PTranslationItemId, item.PTranslationItemHeadword, item.PTranslationItemLanguage);
            card.PCardTranslationClear();
            PTranslationMenuHide();
            return;
        }

        LEntry created;
        try
        {
            created = _lEngine.LEngineTranslationCreate(
                item.PTranslationItemHeadword, item.PTranslationItemLanguage);
        }
        catch (Exception exception)
        {
            PTranslationMenuHide();
            _pEditorHost.PWindowFailureShow(PTranslationFailureKey, exception);
            return;
        }

        _pTranslationFresh.Add(created.LEntryId);
        card.PCardTranslationCommit(created.LEntryId, created.LEntryHeadword, created.LEntryLanguage);
        card.PCardTranslationClear();
        PTranslationMenuHide();
    }

    private bool PTranslationResolve(PCard card, string text, bool offered)
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
            _pEditorHost.PWindowFailureShow(PTranslationFailureKey, exception);
            return false;
        }

        if (single is not null)
        {
            return card.PCardTranslationCommit(
                single.LEntryId, single.LEntryHeadword, single.LEntryLanguage);
        }

        if (offered)
        {
            PTranslationMenuShow(card, word, found);
        }

        return false;
    }

    private void PTranslationMenuShow(PCard card, string word, IReadOnlyList<LEntry> found)
    {
        _pTranslationItem.Clear();
        foreach (LEntry entry in found)
        {
            _pTranslationItem.Add(new PTranslationItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage, false));
        }

        foreach (string language in PTranslationLangcodeRead())
        {
            _pTranslationItem.Add(new PTranslationItem(string.Empty, word, language, true));
        }

        _pTranslationCard = card;
        PTranslationMenuChoice.PlacementTarget = PTranslationBoxFind(card) ?? (UIElement)PContents;
        PTranslationMenuChoice.IsOpen = true;
        PTranslationMenuList.SelectedIndex = 0;
    }

    private void PTranslationMenuHide()
    {
        PTranslationMenuChoice.IsOpen = false;
        PTranslationMenuList.SelectedIndex = -1;
        _pTranslationItem.Clear();
        _pTranslationCard = null;
    }

    private IReadOnlyList<string> PTranslationLangcodeRead()
    {
        List<string> languages = [];
        foreach (PLangcodeItem item in _pLangcodeItem)
        {
            if (!string.Equals(item.PLangcodeItemName, _pLangcodeChoice, StringComparison.Ordinal))
            {
                languages.Add(item.PLangcodeItemName);
            }
        }

        languages.Add(_pLangcodeChoice);
        return languages;
    }

    private TextBox? PTranslationBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PTranslationEntry row } box &&
            PCardTranslationFind(row) == card
            ? box
            : null;
    }

    private static void PTranslationEntryApply(TextBox box, PTranslationEntry row, int caret)
    {
        ItemsControl? host = ItemsControl.ItemsControlFromItemContainer(box) ?? PTranslationHostFind(box);
        box.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                TextBox? entry = host is null ? box : PTranslationEntryFind(host) ?? box;
                if (entry.DataContext != row)
                {
                    return;
                }

                entry.Focus();
                entry.CaretIndex = caret > entry.Text.Length ? entry.Text.Length : caret;
            });
    }

    private static ItemsControl? PTranslationHostFind(DependencyObject start)
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

    private static TextBox? PTranslationEntryFind(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            if (child is TextBox { DataContext: PTranslationEntry } box)
            {
                return box;
            }

            TextBox? found = PTranslationEntryFind(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private PCard? PCardTranslationFind(object row)
    {
        foreach (PCard card in _pSenseList)
        {
            if (card.PCardTranslation.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardTranslation.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
