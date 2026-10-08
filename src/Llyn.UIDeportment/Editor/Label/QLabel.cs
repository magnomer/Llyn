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

    internal QLabel(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qLabelSurface = surface;
        _qLabelMeaning = meaning;
        _qLabelCollocation = collocation;
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QLabelTextObserve));
    }

    internal void QLabelIntroduce(CCard card, QSlate slate)
    {
        _cCard = card;
        _qLabelSlate = slate;
    }

    internal static void QLabelShow(PCard card, IReadOnlyList<CTagDraft> drafts)
    {
        List<PLabelChip> chips = [];
        foreach (CTagDraft draft in drafts)
        {
            chips.Add(new PLabelChip(draft.CTagDraftId, draft.CTagDraftText));
        }

        card.PCardLabel.PCaretShow(chips);
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
            _qLabelSlate.QSlateRefine(card, _cCard.CCardTagAdd(
                card.PCardId, box.Text, card.PCardLabel.PCaretPosition, false));
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
            || sender is not FrameworkElement { DataContext: PCaret<PLabelChip> row }
            || QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        _cCard.CCardTagAdd(
            card.PCardId, row.PCaretText, row.PCaretPosition, true);
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
            step =>
            {
                if (row.PCaretFind(step) is PLabelChip chip)
                {
                    _cCard.CCardTagRemove(card.PCardId, chip.PLabelChipId);
                }
            });
    }

    internal void QLabelCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<PLabelChip> row } box
            || QLabelCardFind(row) is null)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            row.PCaretMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QLabelBlurRefine(object sender, RoutedEventArgs e)
    {
        _qLabelSlate.QSlateShutRefine();
    }

    internal void QLabelCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCaret<PLabelChip> row }
            && QLabelCardFind(row) is PCard card)
        {
            _cCard.CCardTagAdd(
                card.PCardId, row.PCaretText, row.PCaretPosition, true);
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
