using System;
using System.Collections.Generic;
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

    internal void PLinkAttach(PCard card)
    {
        card.PCardLinkDispatcher = (text, offered) => PLinkResolve(card, text, offered);
        card.PCardLinkNotice = text => PLinkProspectShow(card, text);
    }

    private void PLinkProspectShow(PCard card, string text)
    {
        string word = (text ?? string.Empty).Trim();
        if (word.Length == 0)
        {
            PProspectHide();
            return;
        }

        IReadOnlyList<LEntry> found;
        try
        {
            found = _lEngine.LEngineTranslationFind(word, PEditorEntryRead());
        }
        catch (Exception)
        {
            PProspectHide();
            return;
        }

        PProspectShow(card, word, found, false);
    }

    internal void PLinkChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLinkChip chip } && PCardLinkFind(chip) is PCard card)
        {
            PLinkRemove(card, chip);
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

        if (PProspect.IsOpen && PProspectHandle(e.Key))
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
            PLinkRemove(card, card.PCardLinkFind(-1));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            PLinkRemove(card, card.PCardLinkFind(1));
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
        if (sender is not FrameworkElement { DataContext: PLinkCaret row })
        {
            return;
        }

        PProspectHide();

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

    internal void PLinkFlagUpdate()
    {
        foreach (PCard card in _pMeaningList)
        {
            card.PCardFlagUpdate();
        }

        foreach (PCard card in _pCollocationList)
        {
            card.PCardFlagUpdate();
        }
    }

    private void PLinkRemove(PCard card, PLinkChip? chip)
    {
        if (chip is null)
        {
            return;
        }

        PEditorRequestSend(new LRequestTranslationRemoval(_pEditorDraft, card.PCardId, chip.PLinkChipId));
        PLinkCourtDelete(chip.PLinkChipId);
    }

    private bool PLinkSend(PCard card, long id)
    {
        if (id == 0)
        {
            return false;
        }

        if (!card.PCardLinkCheck(id))
        {
            PEditorRequestSend(new LRequestTranslationPick(_pEditorDraft, card.PCardId, id, card.PCardLinkPosition));
        }

        return true;
    }

    private void PLinkCourtDelete(long id)
    {
        if (_pEditorDraft == 0)
        {
            return;
        }

        try
        {
            LCourt? link = _lEngine.LEngineCourtFind(_pEditorDraft, id);
            if (link is null)
            {
                return;
            }

            _lEngine.LEngineCourtDelete(link.LCourtId);
            _lEngine.LEngineDraftDelete(id);
        }
        catch (Exception)
        {
        }
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
            long? entry = PEditorEntryRead();
            single = _lEngine.LEngineTranslationResolve(word, entry);
            found = single is null || offered ? _lEngine.LEngineTranslationFind(word, entry) : [];
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow(PLinkFailureKey, exception);
            return false;
        }

        if (single is not null && !offered)
        {
            PProspectHide();
            return PLinkSend(card, single.LEntryId);
        }

        if (offered)
        {
            PProspectShow(card, word, found, single is not null);
        }

        return false;
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
        foreach (PCard card in _pMeaningList)
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
