using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PContextAttach(PCard card)
    {
        card.PCardContextNotice = text => PCandidateShow(card, text);
        card.PCardContextDispatcher = text => PContextSend(card, text);
    }

    internal void PContextChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext chip } && PCardContextFind(chip) is PCard card)
        {
            PContextRemove(card, chip);
        }
    }

    internal void PContextCaretHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PContextCaret row } box)
        {
            return;
        }

        PCard? card = PCardContextFind(row);
        if (card is null)
        {
            return;
        }

        if (PCandidate.IsOpen && PCandidateHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            PContextCommit(card);
            e.Handled = true;
            return;
        }

        if (box.SelectionLength != 0)
        {
            return;
        }

        if (e.Key == Key.Back && box.CaretIndex == 0)
        {
            PContextRemove(card, card.PCardContextFind(-1));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            PContextRemove(card, card.PCardContextFind(1));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Left && box.Text.Length == 0 && card.PCardContextMove(-1))
        {
            PEditorCaretApply(box, row, 0);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && box.Text.Length == 0 && card.PCardContextMove(1))
        {
            PEditorCaretApply(box, row, 0);
            e.Handled = true;
        }
    }

    internal void PContextCloseHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PContextCaret row })
        {
            return;
        }

        PCard? card = PCardContextFind(row);
        if (card is not null)
        {
            PContextCommit(card);
        }
    }

    internal void PContextFocusHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = PEditorCaretFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    private void PContextCommit(PCard card)
    {
        PCandidateHide();
        PContextSend(card, card.PCardContextText);
        card.PCardContextClear();
    }

    private bool PContextSend(PCard card, string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || card.PCardContextCheck(written))
        {
            return false;
        }

        PContextSend(card, null, written);
        return true;
    }

    private void PContextSend(PCard card, long? id, string written)
    {
        int position = card.PCardContextPosition;
        PEditorRequestSend(id is long picked
            ? new LRequestSituationPick(PEditorDraft, card.PCardId, picked, position)
            : new LRequestSituationAddition(PEditorDraft, card.PCardId, new LStateWritten(written), position));
    }

    private void PContextRemove(PCard card, PContext? chip)
    {
        if (chip is not null)
        {
            PEditorRequestSend(new LRequestSituationRemoval(PEditorDraft, card.PCardId, chip.PContextId));
        }
    }

    private PCard? PCardContextFind(object row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardContext.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardContext.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
