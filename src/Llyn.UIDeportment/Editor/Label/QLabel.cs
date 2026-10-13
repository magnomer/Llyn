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

    private readonly ObservableCollection<PCard> _qLabelMeaning;

    private readonly ObservableCollection<PCard> _qLabelCollocation;

    private QSlate _qLabelSlate = null!;

    private CCard _cCard = null!;

    private CCaret _cCaret = null!;

    internal QLabel(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
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

    internal void QLabelShow(PCard card, CCardDraft draft, long? anchor)
    {
        List<PLabelChip> chips = [];
        foreach (CTagDraft tag in draft.CCardDraftTag)
        {
            chips.Add(new PLabelChip(tag.CTagDraftId, tag.CTagDraftText));
        }

        card.PCardLabel.PCaretRowShow(chips);
        QLabelAnchorShow(card, anchor);
    }

    private static void QLabelAnchorShow(PCard card, long? anchor)
    {
        PLabelChip? paired = null;
        foreach (PLabelChip chip in card.PCardLabel.PCaretRow)
        {
            if (chip.PLabelChipId == anchor)
            {
                paired = chip;
                break;
            }
        }

        card.PCardLabel.PCaretAnchorShow(paired);
    }

    internal void QLabelApply(FrameworkElement container, object item, string? _)
    {
        if (item is PCaret<PLabelChip> caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PLabelEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PCaretHint);
                entry.Text = caret.PCaretText;
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

        if (item is not PLabelChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLabelName") is TextBlock name)
        {
            name.Text = chip.PLabelChipName;
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

    internal void QLabelFieldApply(ItemsControl list, PCard card)
    {
        list.ItemTemplateSelector ??= new PLabelSelector
        {
            PLabelSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qLabelSurface, "Theme.Label.Chip"),
            PLabelSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qLabelSurface, "Theme.Label.Entry"),
        };
        QLookItem.QLookItemAttach(list, QLabelApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(list, card.PCardLabel, nameof(PCaret<PLabelChip>.PCaretAnchor)),
            QLabelApply);
        if (QLook.QLookPartFind<Border>(list, "PLabelFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QLabelFocusRefine;
            frame.MouseLeftButtonDown += QLabelFocusRefine;
        }
    }

    private void QLabelTextObserve(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: PCaret<PLabelChip> caret } box
            && QLabelCardFind(caret) is PCard card)
        {
            _qLabelSlate.QSlateRefine(card, _cCaret.CCaretTagAdd(card.PCardId, box.Text));
        }
    }

    internal void QLabelChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelChip chip } && QLabelCardFind(chip) is PCard card)
        {
            _cCard.CCardTagRemove(card.PCardId, chip.PLabelChipId);
        }
    }

    internal void QLabelCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: PCaret<PLabelChip> row } box
            || QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        _cCaret.CCaretTagSettle(card.PCardId, box.Text);
        e.Handled = true;
        _qLabelSlate.QSlateShutRefine();
        row.PCaretClear();
    }

    internal void QLabelEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<PLabelChip> row } box
            || QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretEdgeApply(
            e.Key.ToString(),
            box.CaretIndex,
            box.Text.Length,
            box.SelectionLength,
            step => _cCaret.CCaretTag.CCaretTrailDelete(card.PCardId, step));
    }

    internal void QLabelCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<PLabelChip> row } box
            || QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            step =>
            {
                (bool moved, long? anchor) = _cCaret.CCaretTag.CCaretTrailMove(card.PCardId, step);
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
        if (sender is TextBox { DataContext: PCaret<PLabelChip> row } box
            && QLabelCardFind(row) is PCard card)
        {
            _cCaret.CCaretTagSettle(card.PCardId, box.Text);
            row.PCaretClear();
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

    internal PCard? QLabelCardFind(object row)
    {
        foreach (PCard card in _qLabelMeaning)
        {
            if ((row is PLabelChip chip && card.PCardLabel.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardLabel, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qLabelCollocation)
        {
            if ((row is PLabelChip chip && card.PCardLabel.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardLabel, row))
            {
                return card;
            }
        }

        return null;
    }
}
