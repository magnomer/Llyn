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
            if (QLook.QLookPartFind<TextBox>(container, "PRegisterEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PRegisterCaretHint);
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

        if (QLook.QLookPartFind<TextBlock>(container, "PRegisterName") is TextBlock name)
        {
            name.Text = (string)new QStateConverter().Convert(
                [chip.PRegisterText, QLocalizationCatalog.QLocalizationTextRead("Display.Unknown")],
                typeof(string),
                string.Empty,
                CultureInfo.CurrentCulture);
        }

        if (QLook.QLookPartFind<Button>(container, "PRegisterEraser") is Button eraser)
        {
            eraser.Click -= _pRegisterTemplate.PRegisterChipHandle;
            eraser.Click += _pRegisterTemplate.PRegisterChipHandle;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PRegisterIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
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
        QLookItem.QLookItemAttach(list, PRegisterApply);
        if (QLook.QLookPartFind<Border>(list, "PRegisterFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= _pRegisterTemplate.PRegisterFocusHandle;
            frame.MouseLeftButtonDown += _pRegisterTemplate.PRegisterFocusHandle;
        }
    }

    internal void PRegisterAttach(PCard card)
    {
        card.PCardRegisterNotice = text => PCandidateRegisterShow(card, text);
    }

    private void PRegisterTextObserve(PRegisterCaret caret, string text)
    {
        if (PCardRegisterFind(caret) is PCard card)
        {
            card.PCardRegisterRefine(_lEditor.LEditorStudio.CEditorCard.CCardRegisterAdd(
                card.PCardId, text, card.PCardRegisterPosition, false));
        }
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
            PRegisterCommitObserve(card);
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
            PRegisterCommitObserve(card);
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

    private void PRegisterCommitObserve(PCard card)
    {
        PCandidateHide();
        _lEditor.LEditorStudio.CEditorCard.CCardRegisterAdd(
            card.PCardId, card.PCardRegisterText, card.PCardRegisterPosition, true);
        card.PCardRegisterClear();
    }

    private void PRegisterRemove(PCard card, PRegister? chip)
    {
        if (chip is not null)
        {
            PEditorRequestSend(new LRequestRegisterRemoval(PEditorDraft, card.PCardId, chip.PRegisterId));
        }
    }
}
