using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PRegisterTemplate _pRegisterTemplate;

    private void PRegisterApply(FrameworkElement container, object item, string? _)
    {
        if (item is PRegisterCaret caret)
        {
            if (PLook.PLookPartFind<TextBox>(container, "PRegisterEntry") is TextBox entry)
            {
                entry.Tag = caret.PRegisterCaretHint;
                entry.Text = caret.PRegisterCaretText;
                entry.PreviewKeyDown -= _pRegisterTemplate.PRegisterCaretHandle;
                entry.PreviewKeyDown += _pRegisterTemplate.PRegisterCaretHandle;
                entry.LostKeyboardFocus -= _pRegisterTemplate.PRegisterCloseHandle;
                entry.LostKeyboardFocus += _pRegisterTemplate.PRegisterCloseHandle;
            }

            return;
        }

        if (item is not PRegister chip)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PRegisterName") is TextBlock name)
        {
            name.Text = (string)new PStateConverter().Convert(
                [chip.PRegisterText, PLocalizationCatalog.PLocalizationTextRead("Display.Unknown")],
                typeof(string),
                string.Empty,
                CultureInfo.CurrentCulture);
        }

        if (PLook.PLookPartFind<Button>(container, "PRegisterEraser") is Button eraser)
        {
            eraser.Click -= _pRegisterTemplate.PRegisterChipHandle;
            eraser.Click += _pRegisterTemplate.PRegisterChipHandle;
            if (PLook.PLookPartFind<PIconImage>(eraser, "PRegisterIcon") is PIconImage icon)
            {
                icon.PIconSource = PIcon.PIconResolve("close", 12);
            }
        }
    }

    private void PRegisterFieldApply(ItemsControl list)
    {
        list.ItemTemplateSelector ??= new PRegisterSelector
        {
            PRegisterSelectorChip = (DataTemplate)_pRegisterTemplate["Theme.Register.Chip"],
            PRegisterSelectorCaret = (DataTemplate)_pRegisterTemplate["Theme.Register.Entry"],
        };
        PLookItem.PLookItemAttach(list, PRegisterApply);
        if (PLook.PLookPartFind<Border>(list, "PRegisterFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= _pRegisterTemplate.PRegisterFocusHandle;
            frame.MouseLeftButtonDown += _pRegisterTemplate.PRegisterFocusHandle;
        }
    }

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

        e.Handled = PCaretKeyApply(
            box,
            e.Key,
            step => PRegisterRemove(card, card.PCardRegisterFind(step)),
            card.PCardRegisterMove,
            () => PEditorCaretApply(box, row, 0));
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
