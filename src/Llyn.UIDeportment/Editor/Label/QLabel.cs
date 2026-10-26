using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLabel
{
    private readonly FrameworkElement _qLabelSurface;

    private readonly ObservableCollection<QPlacard> _qLabelMeaning;

    private readonly ObservableCollection<QPlacard> _qLabelCollocation;

    private QSlate _qLabelSlate = null!;

    private CCard _cCard = null!;

    private CCaret _cCaret = null!;

    internal QLabel(
        FrameworkElement surface, ObservableCollection<QPlacard> meaning, ObservableCollection<QPlacard> collocation)
    {
        _qLabelSurface = surface;
        _qLabelMeaning = meaning;
        _qLabelCollocation = collocation;
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QLabelTextObserve));
    }

    internal void QLabelIntroduce(CCard card, CCaret caret, QSlate slate)
    {
        _cCard = card;
        _cCaret = caret;
        _qLabelSlate = slate;
    }

    internal void QLabelShow(QPlacard card, CCardDraft draft, long? anchor)
    {
        List<QLabelChip> chips = [];
        foreach (CTagDraft tag in draft.CCardDraftTag)
        {
            chips.Add(new QLabelChip(tag.CTagDraftId, tag.CTagDraftText));
        }

        card.QPlacardLabel.QInletRowShow(chips);
        QLabelAnchorShow(card, anchor);
    }

    private static void QLabelAnchorShow(QPlacard card, long? anchor)
    {
        QLabelChip? paired = null;
        foreach (QLabelChip chip in card.QPlacardLabel.QInletRow)
        {
            if (chip.QLabelChipId == anchor)
            {
                paired = chip;
                break;
            }
        }

        card.QPlacardLabel.QInletAnchorShow(paired);
    }

    internal void QLabelApply(FrameworkElement container, object item, string? _)
    {
        if (item is QInlet<QLabelChip> caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PLabelEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.QInletHint);
                entry.Text = caret.QInletText;
                entry.PreviewKeyDown -= _qLabelSlate.QSlateKeyRefine;
                entry.PreviewKeyDown -= _qLabelSlate.QSlateKeyObserve;
                entry.PreviewKeyDown -= QLabelCommitObserve;
                entry.PreviewKeyDown -= QLabelEraseObserve;
                entry.PreviewKeyDown -= QLabelCaretRefine;
                entry.PreviewKeyDown += _qLabelSlate.QSlateKeyRefine;
                entry.PreviewKeyDown += _qLabelSlate.QSlateKeyObserve;
                entry.PreviewKeyDown += QLabelCommitObserve;
                entry.PreviewKeyDown += QLabelEraseObserve;
                entry.PreviewKeyDown += QLabelCaretRefine;
                entry.LostKeyboardFocus -= QLabelBlurRefine;
                entry.LostKeyboardFocus -= QLabelCloseObserve;
                entry.LostKeyboardFocus += QLabelBlurRefine;
                entry.LostKeyboardFocus += QLabelCloseObserve;
            }

            return;
        }

        if (item is not QLabelChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLabelName") is TextBlock name)
        {
            name.Text = chip.QLabelChipName;
        }

        if (QLook.QLookPartFind<Button>(container, "PLabelEraser") is Button eraser)
        {
            eraser.Click -= QLabelChipObserve;
            eraser.Click += QLabelChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PLabelIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    internal void QLabelFieldApply(ItemsControl list, QPlacard card)
    {
        list.ItemTemplateSelector ??= new QLabelSelector
        {
            QLabelSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qLabelSurface, "Theme.Label.Chip"),
            QLabelSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qLabelSurface, "Theme.Label.Entry"),
        };
        QLookItem.QLookItemAttach(list, QLabelApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(list, card.QPlacardLabel, nameof(QInlet<QLabelChip>.QInletAnchor)),
            QLabelApply);
        if (QLook.QLookPartFind<Border>(list, "PLabelFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QLabelFocusRefine;
            frame.MouseLeftButtonDown += QLabelFocusRefine;
        }
    }

    private void QLabelTextObserve(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: QInlet<QLabelChip> caret } box
            && QLabelCardFind(caret) is QPlacard card)
        {
            _qLabelSlate.QSlateRefine(
                card, _cCaret.CCaretTagAdd(card.QPlacardCollocated, card.QPlacardId, box.Text));
        }
    }

    internal void QLabelChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QLabelChip chip } && QLabelCardFind(chip) is QPlacard card)
        {
            _cCard.CCardTagRemove(card.QPlacardCollocated, card.QPlacardId, chip.QLabelChipId);
        }
    }

    internal void QLabelCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: QInlet<QLabelChip> row } box
            || QLabelCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = true;
        _qLabelSlate.QSlateRefine(
            card, _cCaret.CCaretTagSettle(card.QPlacardCollocated, card.QPlacardId, box.Text));
    }

    internal void QLabelEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: QInlet<QLabelChip> row } box
            || QLabelCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretEdgeApply(
            e.Key.ToString(),
            box.CaretIndex,
            box.Text.Length,
            box.SelectionLength,
            step => _cCaret.CCaretTag.CCaretTrailDelete(card.QPlacardCollocated, card.QPlacardId, step));
    }

    internal void QLabelCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: QInlet<QLabelChip> row } box
            || QLabelCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            step =>
            {
                (bool moved, long? anchor) =
                    _cCaret.CCaretTag.CCaretTrailMove(card.QPlacardCollocated, card.QPlacardId, step);
                QLabelAnchorShow(card, anchor);
                return moved;
            },
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QLabelBlurRefine(object sender, RoutedEventArgs e)
    {
        _qLabelSlate.QSlateShutRefine();
    }

    internal void QLabelCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox { DataContext: QInlet<QLabelChip> row } box
            && QLabelCardFind(row) is QPlacard card)
        {
            card.QPlacardLabel.QInletRefine(
                _cCaret.CCaretTagSettle(card.QPlacardCollocated, card.QPlacardId, box.Text).CProfferText);
        }
    }

    internal void QLabelFocusRefine(object sender, MouseButtonEventArgs e)
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

    internal QPlacard? QLabelCardFind(object row)
    {
        foreach (QPlacard card in _qLabelMeaning)
        {
            if ((row is QLabelChip chip && card.QPlacardLabel.QInletRow.Contains(chip))
                || ReferenceEquals(card.QPlacardLabel, row))
            {
                return card;
            }
        }

        foreach (QPlacard card in _qLabelCollocation)
        {
            if ((row is QLabelChip chip && card.QPlacardLabel.QInletRow.Contains(chip))
                || ReferenceEquals(card.QPlacardLabel, row))
            {
                return card;
            }
        }

        return null;
    }
}
