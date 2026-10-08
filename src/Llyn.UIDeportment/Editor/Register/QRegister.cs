using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRegister
{
    private readonly FrameworkElement _qRegisterSurface;

    private readonly ObservableCollection<PCard> _qRegisterMeaning;

    private readonly ObservableCollection<PCard> _qRegisterCollocation;

    private CCard _cCard = null!;

    private QProffer _qRegisterProffer = null!;

    internal QRegister(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qRegisterSurface = surface;
        _qRegisterMeaning = meaning;
        _qRegisterCollocation = collocation;
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QRegisterTextObserve));
    }

    internal void QRegisterIntroduce(CCard card, QProffer proffer)
    {
        _cCard = card;
        _qRegisterProffer = proffer;
    }

    internal static void QRegisterShow(PCard card, IReadOnlyList<CRegisterDraft> drafts)
    {
        List<PRegister> chips = [];
        foreach (CRegisterDraft draft in drafts)
        {
            chips.Add(new PRegister(draft.CRegisterDraftName, draft.CRegisterDraftId));
        }

        card.PCardRegister.PCaretShow(chips);
    }

    internal void QRegisterApply(FrameworkElement container, object item, string? _)
    {
        if (item is PCaret<PRegister> caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PRegisterEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PCaretHint);
                entry.Text = caret.PCaretText;
                entry.PreviewKeyDown -= _qRegisterProffer.QProfferKeyRefine;
                entry.PreviewKeyDown -= _qRegisterProffer.QProfferKeyObserve;
                entry.PreviewKeyDown -= QRegisterCommitObserve;
                entry.PreviewKeyDown -= QRegisterEraseObserve;
                entry.PreviewKeyDown -= QRegisterCaretRefine;
                entry.PreviewKeyDown += _qRegisterProffer.QProfferKeyRefine;
                entry.PreviewKeyDown += _qRegisterProffer.QProfferKeyObserve;
                entry.PreviewKeyDown += QRegisterCommitObserve;
                entry.PreviewKeyDown += QRegisterEraseObserve;
                entry.PreviewKeyDown += QRegisterCaretRefine;
                entry.LostKeyboardFocus -= QRegisterBlurRefine;
                entry.LostKeyboardFocus -= QRegisterCloseObserve;
                entry.LostKeyboardFocus += QRegisterBlurRefine;
                entry.LostKeyboardFocus += QRegisterCloseObserve;
            }

            return;
        }

        if (item is not PRegister chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRegisterName") is TextBlock name)
        {
            QStateConverter.QStateTextRefine(name, TextBlock.TextProperty, chip.PRegisterText);
        }

        if (QLook.QLookPartFind<Button>(container, "PRegisterEraser") is Button eraser)
        {
            eraser.Click -= QRegisterChipObserve;
            eraser.Click += QRegisterChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PRegisterIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    internal void QRegisterFieldApply(ItemsControl list, PCard card)
    {
        list.ItemTemplateSelector ??= new PRegisterSelector
        {
            PRegisterSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qRegisterSurface, "Theme.Register.Chip"),
            PRegisterSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qRegisterSurface, "Theme.Register.Entry"),
        };
        QLookItem.QLookItemAttach(list, QRegisterApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(
                list, card.PCardRegister, nameof(PCaret<PRegister>.PCaretAnchor)),
            QRegisterApply);
        if (QLook.QLookPartFind<Border>(list, "PRegisterFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QRegisterFocusRefine;
            frame.MouseLeftButtonDown += QRegisterFocusRefine;
        }
    }

    private void QRegisterTextObserve(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: PCaret<PRegister> caret } box
            && QRegisterCardFind(caret) is PCard card)
        {
            _qRegisterProffer.QProfferRegisterRefine(card, _cCard.CCardRegisterAdd(
                card.PCardId, box.Text, card.PCardRegister.PCaretPosition, false));
        }
    }

    internal void QRegisterChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PRegister chip } && QRegisterCardFind(chip) is PCard card)
        {
            _cCard.CCardRegisterRemove(card.PCardId, chip.PRegisterId);
        }
    }

    internal void QRegisterCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PCaret<PRegister> row }
            || QRegisterCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = true;
        _qRegisterProffer.QProfferRegisterRefine(card, _cCard.CCardRegisterAdd(
            card.PCardId, card.PCardRegister.PCaretText, card.PCardRegister.PCaretPosition, true));
    }

    internal void QRegisterEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<PRegister> row } box
            || QRegisterCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretEdgeApply(
            e.Key.ToString(),
            box.CaretIndex,
            box.Text.Length,
            box.SelectionLength,
            step =>
            {
                if (card.PCardRegister.PCaretFind(step) is PRegister chip)
                {
                    _cCard.CCardRegisterRemove(card.PCardId, chip.PRegisterId);
                }
            });
    }

    internal void QRegisterCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<PRegister> row } box
            || QRegisterCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardRegister.PCaretMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QRegisterBlurRefine(object sender, RoutedEventArgs e)
    {
        _qRegisterProffer.QProfferShutRefine();
    }

    internal void QRegisterCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCaret<PRegister> row }
            && QRegisterCardFind(row) is PCard card)
        {
            _qRegisterProffer.QProfferRegisterRefine(card, _cCard.CCardRegisterAdd(
                card.PCardId, card.PCardRegister.PCaretText, card.PCardRegister.PCaretPosition, true));
        }
    }

    internal void QRegisterFocusRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = QField.QFieldCaretFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    internal PCard? QRegisterCardFind(object row)
    {
        foreach (PCard card in _qRegisterMeaning)
        {
            if ((row is PRegister chip && card.PCardRegister.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardRegister, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qRegisterCollocation)
        {
            if ((row is PRegister chip && card.PCardRegister.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardRegister, row))
            {
                return card;
            }
        }

        return null;
    }
}
