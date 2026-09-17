using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PRegisterAttach(PCard card)
    {
        card.PCardRegisterNotice = text => PCandidateRegisterShow(card, text);
        card.PCardRegisterDispatcher = text => PRegisterSend(card, text);
    }

    internal void PRegisterChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PRegister chip } && PCardRegisterFind(chip) is PCard card)
        {
            PRegisterRemove(card, chip);
        }
    }

    internal void PRegisterCaretHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PRegisterCaret row } box)
        {
            return;
        }

        PCard? card = PCardRegisterFind(row);
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
            PRegisterCommit(card);
            e.Handled = true;
            return;
        }

        if (box.SelectionLength != 0)
        {
            return;
        }

        if (e.Key == Key.Back && box.CaretIndex == 0)
        {
            PRegisterRemove(card, card.PCardRegisterFind(-1));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            PRegisterRemove(card, card.PCardRegisterFind(1));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Left && box.Text.Length == 0 && card.PCardRegisterMove(-1))
        {
            PEditorCaretApply(box, row, 0);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && box.Text.Length == 0 && card.PCardRegisterMove(1))
        {
            PEditorCaretApply(box, row, 0);
            e.Handled = true;
        }
    }

    internal void PRegisterCloseHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PRegisterCaret row })
        {
            return;
        }

        PCard? card = PCardRegisterFind(row);
        if (card is not null)
        {
            PRegisterCommit(card);
        }
    }

    internal void PRegisterFocusHandle(object sender, MouseButtonEventArgs e)
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

    internal PCard? PCardRegisterFind(object row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardRegister.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardRegister.Contains(row))
            {
                return card;
            }
        }

        return null;
    }

    private void PRegisterCommit(PCard card)
    {
        PCandidateHide();
        PRegisterSend(card, card.PCardRegisterText);
        card.PCardRegisterClear();
    }

    private bool PRegisterSend(PCard card, string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || card.PCardRegisterCheck(written))
        {
            return false;
        }

        PRegisterSend(card, null, written);
        return true;
    }

    private void PRegisterSend(PCard card, long? id, string written)
    {
        int position = card.PCardRegisterPosition;
        PEditorRequestSend(id is long picked
            ? new LRequestRegisterPick(PEditorDraft, card.PCardId, picked, position)
            : new LRequestRegisterAddition(PEditorDraft, card.PCardId, new LStateWritten(written), position));
    }

    private void PRegisterRemove(PCard card, PRegister? chip)
    {
        if (chip is not null)
        {
            PEditorRequestSend(new LRequestRegisterRemoval(PEditorDraft, card.PCardId, chip.PRegisterId));
        }
    }
}
