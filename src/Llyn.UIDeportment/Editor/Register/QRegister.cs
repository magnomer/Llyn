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

    private readonly ObservableCollection<QPlacard> _qRegisterMeaning;

    private readonly ObservableCollection<QPlacard> _qRegisterCollocation;

    private CCard _cCard = null!;

    private CCaret _cCaret = null!;

    private QProffer _qRegisterProffer = null!;

    internal QRegister(
        FrameworkElement surface, ObservableCollection<QPlacard> meaning, ObservableCollection<QPlacard> collocation)
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

    internal static void QRegisterShow(QPlacard card, IReadOnlyList<CRegisterDraft> drafts, long? anchor)
    {
        List<QRegisterChip> chips = [];
        foreach (CRegisterDraft draft in drafts)
        {
            chips.Add(new QRegisterChip(draft.CRegisterDraftName, draft.CRegisterDraftId));
        }

        card.QPlacardRegister.QInletRowShow(chips);
        QRegisterAnchorShow(card, anchor);
    }

    private static void QRegisterAnchorShow(QPlacard card, long? anchor)
    {
        QRegisterChip? paired = null;
        foreach (QRegisterChip chip in card.QPlacardRegister.QInletRow)
        {
            if (chip.QRegisterChipId == anchor)
            {
                paired = chip;
                break;
            }
        }

        card.QPlacardRegister.QInletAnchorShow(paired);
    }

    internal void QRegisterApply(FrameworkElement container, object item, string? _)
    {
        if (item is QInlet<QRegisterChip> caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PRegisterEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.QInletHint);
                entry.Text = caret.QInletText;
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

        if (item is not QRegisterChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRegisterName") is TextBlock name)
        {
            QStateConverter.QStateTextRefine(name, TextBlock.TextProperty, chip.QRegisterChipText);
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

    internal void QRegisterFieldApply(ItemsControl list, QPlacard card)
    {
        list.ItemTemplateSelector ??= new QRegisterSelector
        {
            QRegisterSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qRegisterSurface, "Theme.Register.Chip"),
            QRegisterSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qRegisterSurface, "Theme.Register.Entry"),
        };
        QLookItem.QLookItemAttach(list, QRegisterApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(
                list, card.QPlacardRegister, nameof(QInlet<QRegisterChip>.QInletAnchor)),
            QRegisterApply);
        if (QLook.QLookPartFind<Border>(list, "PRegisterFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QRegisterFocusRefine;
            frame.MouseLeftButtonDown += QRegisterFocusRefine;
        }
    }

    private void QRegisterTextObserve(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: QInlet<QRegisterChip> caret } box
            && QRegisterCardFind(caret) is QPlacard card)
        {
            _qRegisterProffer.QProfferRegisterRefine(card, _cCaret.CCaretRegisterAdd(card.QPlacardId, box.Text));
        }
    }

    internal void QRegisterChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QRegisterChip chip } && QRegisterCardFind(chip) is QPlacard card)
        {
            _cCard.CCardRegisterRemove(card.QPlacardId, chip.QRegisterChipId);
        }
    }

    internal void QRegisterCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: QInlet<QRegisterChip> row } box
            || QRegisterCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = true;
        _qRegisterProffer.QProfferRegisterRefine(card, _cCaret.CCaretRegisterSettle(card.QPlacardId, box.Text));
    }

    internal void QRegisterEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: QInlet<QRegisterChip> row } box
            || QRegisterCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretEdgeApply(
            e.Key.ToString(),
            box.CaretIndex,
            box.Text.Length,
            box.SelectionLength,
            step => _cCaret.CCaretRegister.CCaretTrailDelete(card.QPlacardId, step));
    }

    internal void QRegisterCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: QInlet<QRegisterChip> row } box
            || QRegisterCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            step =>
            {
                (bool moved, long? anchor) = _cCaret.CCaretRegister.CCaretTrailMove(card.QPlacardId, step);
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
        if (sender is TextBox { DataContext: QInlet<QRegisterChip> row } box
            && QRegisterCardFind(row) is QPlacard card)
        {
            _qRegisterProffer.QProfferRegisterRefine(card, _cCaret.CCaretRegisterSettle(card.QPlacardId, box.Text));
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

    internal QPlacard? QRegisterCardFind(object row)
    {
        foreach (QPlacard card in _qRegisterMeaning)
        {
            if ((row is QRegisterChip chip && card.QPlacardRegister.QInletRow.Contains(chip))
                || ReferenceEquals(card.QPlacardRegister, row))
            {
                return card;
            }
        }

        foreach (QPlacard card in _qRegisterCollocation)
        {
            if ((row is QRegisterChip chip && card.QPlacardRegister.QInletRow.Contains(chip))
                || ReferenceEquals(card.QPlacardRegister, row))
            {
                return card;
            }
        }

        return null;
    }
}
