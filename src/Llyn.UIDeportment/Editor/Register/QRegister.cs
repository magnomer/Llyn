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

    private CCaret _cCaret = null!;

    private QProffer _qRegisterProffer = null!;

    internal QRegister(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qRegisterSurface = surface;
        _qRegisterMeaning = meaning;
        _qRegisterCollocation = collocation;
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QRegisterTextObserve));
    }

    internal void QRegisterIntroduce(CCard card, CCaret caret, QProffer proffer)
    {
        _cCard = card;
        _cCaret = caret;
        _qRegisterProffer = proffer;
    }

    internal static void QRegisterShow(PCard card, IReadOnlyList<CRegisterDraft> drafts, long? anchor)
    {
        List<PRegister> chips = [];
        foreach (CRegisterDraft draft in drafts)
        {
            chips.Add(new PRegister(draft.CRegisterDraftName, draft.CRegisterDraftId));
        }

        card.PCardRegister.PCaretRowShow(chips);
        QRegisterAnchorShow(card, anchor);
    }

    private static void QRegisterAnchorShow(PCard card, long? anchor)
    {
        PRegister? paired = null;
        foreach (PRegister chip in card.PCardRegister.PCaretRow)
        {
            if (chip.PRegisterId == anchor)
            {
                paired = chip;
                break;
            }
        }

        card.PCardRegister.PCaretAnchorShow(paired);
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
            _qRegisterProffer.QProfferRegisterRefine(card, _cCaret.CCaretRegisterAdd(card.PCardId, box.Text));
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
            || sender is not TextBox { DataContext: PCaret<PRegister> row } box
            || QRegisterCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = true;
        _qRegisterProffer.QProfferRegisterRefine(card, _cCaret.CCaretRegisterSettle(card.PCardId, box.Text));
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
            step => _cCaret.CCaretRegister.CCaretTrailDelete(card.PCardId, step));
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
            step =>
            {
                (bool moved, long? anchor) = _cCaret.CCaretRegister.CCaretTrailMove(card.PCardId, step);
                QRegisterAnchorShow(card, anchor);
                return moved;
            },
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QRegisterBlurRefine(object sender, RoutedEventArgs e)
    {
        _qRegisterProffer.QProfferShutRefine();
    }

    internal void QRegisterCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox { DataContext: PCaret<PRegister> row } box
            && QRegisterCardFind(row) is PCard card)
        {
            _qRegisterProffer.QProfferRegisterRefine(card, _cCaret.CCaretRegisterSettle(card.PCardId, box.Text));
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
